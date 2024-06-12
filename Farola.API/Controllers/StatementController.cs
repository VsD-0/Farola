using Farola.API.Infrastructure.Commands;
using Farola.Database.Models;
using Farola.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;

namespace Farola.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatementController : Controller
    {
        private readonly IMediator _mediator;
        private readonly FarolaContext _context;

        public StatementController(IMediator mediator, FarolaContext context)
        {
            _mediator = mediator;
            _context = context; 
        }

        [HttpGet("GetStatementsById/{proId}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> GetStatementsById(int proId)
        {
            var stats = _context.Statements.Where(s => s.ProfessionalId == proId)
                .Select(s => new StatementsViewModel
                {
                    Id = s.Id,
                    Client = $"{_context.Users.FirstOrDefault(u => u.Id == s.ClientId).Surname} {_context.Users.FirstOrDefault(u => u.Id == s.ClientId).Name} {_context.Users.FirstOrDefault(u => u.Id == s.ClientId).Patronymic}",
                    Status = _context.StatementStatuses.FirstOrDefault(u => u.Id == s.StatusId).Name,
                    Comment = s.Comment,
                    Review = _context.Reviews.FirstOrDefault(r => r.StatementId == s.Id).Text,
                    GradeByClient = _context.Reviews.FirstOrDefault(r => r.StatementId == s.Id).Grade,
                    AvgGradeClient = _context.Statements.Where(sc => sc.ClientId == s.ClientId).Average(s => s.Grade),
                    phoneClient = _context.Users.FirstOrDefault(u => u.Id == s.ClientId).PhoneNumber,
                    Grade = s.Grade,
                    DateAdded = s.DateAdded,
                    DateExpiration = s.DateExpiration
                })
                .OrderByDescending(s => s.DateAdded)
                .ToList();

            foreach (var item in stats)
            {
                List<Option<string>> grades = new();
                for (float i = 1; i <= 5; i += (float)0.5)
                {
                    grades.Add(new Option<string> { Value = i.ToString(), Text = i.ToString() });
                }
                item.Grades = grades;
                if (item.Grade != null)
                {
                    var ind = item.Grades.IndexOf(item.Grades.FirstOrDefault(x => x.Value == item.Grade.ToString()));
                    item.Grades[ind].Selected = true;
                }
            }

            return Ok(stats);
        }

        [HttpGet("GetStatementsByClientId/{clientId}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> GetStatementsByClientId(int clientId)
        {
            var stats = _context.Statements.Where(s => s.ClientId == clientId)
                .Select(s => new StatementsViewModel
                {
                    Id = s.Id,
                    Client = $"{_context.Users.FirstOrDefault(u => u.Id == s.ClientId).Surname} {_context.Users.FirstOrDefault(u => u.Id == s.ClientId).Name} {_context.Users.FirstOrDefault(u => u.Id == s.ClientId).Patronymic}",
                    Pro = _context.Users.FirstOrDefault(u => u.Id == s.ProfessionalId),
                    AvgGradePro = _context.Reviews.Join(_context.Statements,
                        r => r.StatementId,
                        sc => sc.Id,
                        (r, sc) => new { Statement = sc, Review = r })
                    .Where(y => y.Statement.ProfessionalId == s.ProfessionalId).Average(r => r.Review.Grade),
                    Status = _context.StatementStatuses.FirstOrDefault(u => u.Id == s.StatusId).Name,
                    Comment = s.Comment,
                    Review = _context.Reviews.FirstOrDefault(r => r.StatementId == s.Id).Text,
                    phoneClient = _context.Users.FirstOrDefault(u => u.Id == s.ClientId).PhoneNumber,
                    GradeByClient = _context.Reviews.FirstOrDefault(r => r.StatementId == s.Id).Grade,
                    DateAdded = s.DateAdded,
                    DateExpiration = s.DateExpiration
                })
                .OrderByDescending(s => s.DateAdded)
                .ToList();

            foreach (var item in stats)
            {
                List<Option<string>> grades = new();
                for (float i = 1; i <= 5; i += (float)0.5)
                {
                    grades.Add(new Option<string> { Value = i.ToString(), Text = i.ToString() });
                }
                item.Grades = grades;
                if (item.GradeByClient != null)
                {
                    var ind = item.Grades.IndexOf(item.Grades.FirstOrDefault(x => x.Value == item.GradeByClient.ToString()));
                    item.Grades[ind].Selected = true;
                }
            }

            return Ok(stats);
        }

        [HttpPost("Send")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> SendStatement([FromBody] SendStatementCommand sendStatementCommand)
        {
            var statement = await _mediator.Send(sendStatementCommand);
            return statement is not null ? Created(nameof(SendStatement), statement) : BadRequest("Не удалось создать заявку");
        }

        [HttpGet("GetStatuses")]
        public async Task<IActionResult> GetStatuses()
        {
            return Ok(_context.StatementStatuses.AsEnumerable());
        }

        [HttpPost("UpdateStatus/{statementId}/{newStatus}")]
        [Authorize(Roles = "1")]
        public async Task UpdateStatus(int statementId, int newStatus)
        {
            var updateStat = _context.Statements.FirstOrDefault(s => s.Id == statementId);
            if (newStatus == 4)
            {
                updateStat.DateExpiration = DateTime.UtcNow;
            }
            updateStat.StatusId = newStatus;
            _context.Statements.Update(updateStat);
            await _context.SaveChangesAsync();
        }

        [HttpPost("SaveGrade/{statementId}/{grade}/{role}")]
        [Authorize(Roles = "1, 2")]
        public async Task SaveGrade(int statementId, float grade, string role)
        {
            if (role == "1")
            {
                var updateStat = _context.Statements.FirstOrDefault(s => s.Id == statementId);
                updateStat.Grade = grade;
                _context.Statements.Update(updateStat);
                await _context.SaveChangesAsync();
            }
            else if (role == "2")
            {
                if (_context.Reviews.Any(r => r.StatementId == statementId))
                {
                    var updateReview = _context.Reviews.FirstOrDefault(s => s.StatementId == statementId);
                    updateReview.Grade = grade;
                    _context.Reviews.Update(updateReview);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.Reviews.Add(new Review
                    {
                        StatementId = statementId,
                        DateAdded = DateTime.UtcNow,
                        Grade = grade
                    });
                    await _context.SaveChangesAsync();
                }
            }
        }

        [HttpPost("SaveComment/{statementId}/{comment}/{role}")]
        [Authorize(Roles = "1, 2")]
        public async Task SaveComment(int statementId, string comment, string role)
        {
            if (role == "1")
            {
                var updateStat = _context.Statements.FirstOrDefault(s => s.Id == statementId);
                updateStat.Comment = comment;
                _context.Statements.Update(updateStat);
                await _context.SaveChangesAsync();
            }
            else if (role == "2")
            {
                if (_context.Reviews.Any(r => r.StatementId == statementId))
                {
                    var updateReview = _context.Reviews.FirstOrDefault(s => s.StatementId == statementId);
                    updateReview.Text = comment;
                    _context.Reviews.Update(updateReview);
                    await _context.SaveChangesAsync();
                }
            }
        }

        [HttpGet("IsExistGrade/{statementId}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> IsExistGrade(int statementId) => 
            Ok(await _context.Reviews.AnyAsync(s => s.StatementId == statementId && s.Grade >= 1));

        [HttpGet("IsExistActive/{clientId}/{proId}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> IsExistActive(int clientId, int proId) => 
            Ok(await _context.Statements.AnyAsync(s => s.ClientId == clientId && s.ProfessionalId == proId && s.StatusId != 4));


        [HttpPost]
        public async Task<IActionResult> updatePassword(int userId, CancellationToken cancellationToken)
        {
            User user = _context.Users.FirstOrDefault(u => u.Id == userId);

            var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password, salt);
            user.Password = passwordHash;

            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;

            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);

            var refreshTokenEntry = new RefreshToken
            {
                Userid = user.Id,
                Createdat = DateTime.UtcNow,
                Expiresat = DateTime.UtcNow.AddDays(30),
                Token = user.RefreshToken
            };

            await _context.RefreshTokens.AddAsync(refreshTokenEntry, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Ok();
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            return Convert.ToBase64String(randomNumber);
        }
    }
}
