using LoanApp_REST_APIs.Models;
using LoanApp_REST_APIs.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoanApp_REST_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelpController : ControllerBase
    {
        private readonly LoanManagementDbContext _context;

        public HelpController(LoanManagementDbContext context)
        {
            _context = context;
        }

        // GET: api/help
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HelpViewModel>>> GetHelp()
        {
            var helpEntries = await _context.Helps
                .Select(h => new HelpViewModel
                {
                    HelpId = h.HelpId,
                    Title = h.Title,
                    Content = h.Content,
                    CreatedAt = h.CreatedAt,
                    UpdatedAt = h.UpdatedAt
                })
                .ToListAsync();

            return Ok(helpEntries);
        }

        // PUT: api/help/1
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<HelpViewModel>> UpdateHelp(int id, [FromBody] HelpViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var help = await _context.Helps
                .FirstOrDefaultAsync(h => h.HelpId == id);

            if (help == null)
                return NotFound("Help entry not found.");

            help.Title = model.Title;
            help.Content = model.Content;
            help.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            var result = new HelpViewModel
            {
                HelpId = help.HelpId,
                Title = help.Title,
                Content = help.Content,
                CreatedAt = help.CreatedAt,
                UpdatedAt = help.UpdatedAt
            };

            return Ok(result);
        }

        // POST: api/help
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<HelpViewModel>> CreateHelp([FromBody] HelpViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var help = new Help
            {
                Title = model.Title,
                Content = model.Content,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Helps.Add(help);
            await _context.SaveChangesAsync();

            var result = new HelpViewModel
            {
                HelpId = help.HelpId,
                Title = help.Title,
                Content = help.Content,
                CreatedAt = help.CreatedAt,
                UpdatedAt = help.UpdatedAt
            };

            return CreatedAtAction(nameof(GetHelp), new { id = result.HelpId }, result);
        }
    }
}