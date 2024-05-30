using Farola.API.Infrastructure.Commands;
using Farola.Database.Models;
using Farola.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            return Ok(_context.Statements.Where(s => s.ProfessionalId == proId)
                .Select(s => new StatementsViewModel
                {
                    Id = s.Id,
                    Client = $"{_context.Users.FirstOrDefault(u => u.Id == s.ClientId).Surname} {_context.Users.FirstOrDefault(u => u.Id == s.ClientId).Name} {_context.Users.FirstOrDefault(u => u.Id == s.ClientId).Patronymic}",
                    Status =  _context.StatementStatuses.FirstOrDefault(u => u.Id == s.StatusId).Name,
                    Comment = s.Comment,
                    Grade = s.Grade,
                    DateAdded = s.DateAdded,
                    DateExpiration = s.DateExpiration
                }).AsEnumerable());
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

        [HttpPost("SaveGrade/{statementId}/{grade}")]
        [Authorize(Roles = "1")]
        public async Task SaveGrade(int statementId, float grade)
        {
            var updateStat = _context.Statements.FirstOrDefault(s => s.Id == statementId);
            updateStat.Grade = grade;
            _context.Statements.Update(updateStat);
            await _context.SaveChangesAsync();
        }

        [HttpPost("SaveComment/{statementId}/{comment}")]
        [Authorize(Roles = "1")]
        public async Task SaveComment(int statementId, string comment)
        {
            var updateStat = _context.Statements.FirstOrDefault(s => s.Id == statementId);
            updateStat.Comment = comment;
            _context.Statements.Update(updateStat);
            await _context.SaveChangesAsync();
        }

        [HttpGet("IsExistActive/{clientId}/{proId}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> IsExistActive(int clientId, int proId)
        {
            var a = await _context.Statements.AnyAsync(s => s.ClientId == clientId && s.ProfessionalId == proId && s.StatusId != 3);
            return Ok(a);
        }
    }
}
