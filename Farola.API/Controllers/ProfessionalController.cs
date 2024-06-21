using Farola.API.Infrastructure.Commands;
using Farola.API.Infrastructure.Queries;
using Farola.Database.Models;
using Farola.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Net;
using System.Xml.Linq;


namespace Farola.API.Controllers
{
    /// <summary>
    /// Контроллер профессионалов
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProfessionalController : Controller
    {
        private readonly IMediator _mediator;
        private readonly FarolaContext _context;
        private readonly IConfiguration _config;

        public ProfessionalController(IMediator mediator, FarolaContext context, IConfiguration config)
        {
            _mediator = mediator;
            _context = context;
            _config = config;
        }

        [HttpGet("GetProfessionals")]
        public async Task<IActionResult> GetProfessionals(int pageNumber, int pageSize, string? profession, string? specialization)
        {
            return Ok(await _mediator.Send(new GetProfessionalsQuerie { PageNumber = pageNumber, PageSize = pageSize, Profession = profession, Specialization = specialization }));
        }

        [HttpGet("GetProfessional/{id}")]
        public async Task<IActionResult> GetProfessional(int id)
        {
            return Ok(await _context.Users.Where(u => u.Id == id)
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    Name = u.Name,
                    Area = u.Area,
                    Email = u.Email,
                    Information = u.Information,
                    PhoneNumber = u.PhoneNumber,
                    Photo = u.Photo,
                    Profession = u.Profession,
                    RoleId = u.RoleId,
                    Surname = u.Surname,
                    Patronymic = u.Patronymic,
                    SpecializationId = u.SpecializationId,
                    Specialization = _context.Specializations.FirstOrDefault(s => s.Id == u.SpecializationId)!.Name
                })
                .FirstOrDefaultAsync());
        }

        [HttpGet("GetSpecializations")]
        public async Task<IActionResult> GetSpecializations()
        {
            return Ok(_context.Specializations.AsEnumerable());
        }

        [HttpGet("GetReviewsByUser/{userId}")]
        public async Task<IActionResult> GetReviewsByUser(int userId)
        {
            return Ok(_context.Reviews
                .Join(_context.Statements,
                r => r.StatementId,
                s => s.Id,
                (r, s) => new { Review = r, Statement = s })
                .Where(x => x.Statement.ProfessionalId == userId)
                .Select(x => new ReviewViewModel
                {
                    Client = _context.Users.FirstOrDefault(u => u.Id == x.Statement.ClientId),
                    Professional = _context.Users.FirstOrDefault(u => u.Id == x.Statement.ProfessionalId),
                    DateAdded = x.Review.DateAdded,
                    Grade = x.Review.Grade,
                    Text = x.Review.Text
                }).AsEnumerable());
        }

        [HttpGet("GetReviewBestPro")]
        public async Task<IActionResult> GetReviewBestPro(int pageNumber, int pageSize)
        {
            var reviews = _context.Statements
                .Join(_context.Reviews,
                s => s.Id,
                r => r.StatementId,
                (s, r) => new { Statement = s, Review = r })
                .Where(x => x.Review.Grade >= 4)
                .GroupBy(x => x.Statement.ProfessionalId)
                .Select(x => new ReviewViewModel
                {
                    Client = _context.Users.FirstOrDefault(u => u.Id == x.FirstOrDefault().Statement.ClientId),
                    Professional = _context.Users.FirstOrDefault(u => u.Id == x.FirstOrDefault().Statement.ProfessionalId),
                    AvgGrade = _context.Reviews
                    .Join(_context.Statements,
                        r => r.StatementId,
                        s => s.Id,
                        (r, s) => new { Statement = s, Review = r })
                    .Where(y => y.Statement.ProfessionalId == x.FirstOrDefault().Statement.ProfessionalId).Average(r => r.Review.Grade),
                    CountGrade = _context.Reviews
                    .Join(_context.Statements,
                        r => r.StatementId,
                        s => s.Id,
                        (r, s) => new { Statement = s, Review = r })
                    .Where(y => y.Statement.ProfessionalId == x.FirstOrDefault().Statement.ProfessionalId).Count(),
                    Grade = x.FirstOrDefault().Review.Grade,
                    Text = x.FirstOrDefault().Review.Text,
                    DateAdded = x.FirstOrDefault().Review.DateAdded
                }).Where(x => x.AvgGrade >= 4)
                .OrderByDescending(x => x.CountGrade).ThenByDescending(x => x.AvgGrade).ThenByDescending(x => x.Grade).ThenByDescending(x => x.DateAdded);

            var skip = (pageNumber - 1) * pageSize;
            var result = new List<ReviewViewModel>(reviews.Skip(skip).Take(pageSize));

            var pagination = new Pagination<ReviewViewModel>
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalItems = await reviews.CountAsync()
            };

            return Ok(new PaginatedResult<ReviewViewModel>
            {
                Pagination = pagination,
                Items = result
            });
        }


        [HttpGet("GetSpecStats")]
        public async Task<IActionResult> GetSpecStats()
        {
            return Ok(_context.Users.Where(u => u.IsClosed != true)
                .Join(_context.Specializations,
                    u => u.SpecializationId,
                    sp => sp.Id,
                    (u, sp) => new { User = u, Spec = sp })
                .Where(x => x.User.RoleId == 1)
                .GroupBy(x => x.Spec.Name)
                .Select(g => new SpecStat { Spec = g.Select(x => x.Spec).First(), Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .AsEnumerable());
        }

        /// <summary>
        /// Регистрация профессионала
        /// </summary>
        /// <param name="registrationProCommand">Данные пользователя</param>
        /// <returns>Токен</returns>
        /// <response code="201">Успешное выполнение</response>
        /// <response code="400">Ошибка API</response>
        [ProducesResponseType(typeof(UserDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] RegistrationProCommand registrationProCommand)
        {
            var pro = await _mediator.Send(registrationProCommand);
            return pro is not null ? Created(nameof(SignUp), pro) : BadRequest("Ошибка авторизации");
        }

        [HttpPost("SaveImage/{proId}")]
        public async Task<IActionResult> SaveImage(int proId, IFormFile photo)
        {
            string path = Path.Combine(_config.GetValue<string>("FileStorage")!, photo.FileName);
            await using (FileStream fs = new(path, FileMode.Create))
            {
                await photo.OpenReadStream().CopyToAsync(fs);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == proId);
            user.Photo = photo.FileName;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("UpdatePro")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> UpdatePro([FromBody] UserDTO updatedUser)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == updatedUser.Id);
            user.Name = updatedUser.Name;
            user.Surname = updatedUser.Surname;
            user.Patronymic = updatedUser.Patronymic;
            user.Area = updatedUser.Area;
            user.Email = updatedUser.Email;
            user.Information = updatedUser.Information;
            user.PhoneNumber = updatedUser.PhoneNumber;
            user.SpecializationId = updatedUser.SpecializationId;
            user.Profession = updatedUser.Profession;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("IsClosed/{proId}")]
        [Authorize(Roles = "1, 2")]
        public async Task<IActionResult> IsClosed(int proId) =>
            Ok(await _context.Users.Where(u => u.RoleId == 1).AnyAsync(u => u.Id == proId && u.IsClosed == true));

        [HttpPost("UpdateStatus/{proId}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> UpdateStatus(int proId)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == proId);
            user.IsClosed = !user.IsClosed;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
