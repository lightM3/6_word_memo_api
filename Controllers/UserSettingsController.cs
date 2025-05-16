using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WordMemoryApi.Data;
using WordMemoryApi.DTOs;
using WordMemoryApi.Entities;

namespace WordMemoryApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserSettingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserSettingsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<UserSettings>> GetSettings()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var settings = await _context.UserSettings
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (settings == null)
            {
                var newSettings = new UserSettings
                {
                    UserId = userId,
                    DailyWordLimit = 3,
                    LastWordAdditionDate = DateTime.MinValue
                };

                _context.UserSettings.Add(newSettings);
                await _context.SaveChangesAsync();

                return Ok(newSettings);
            }

            return Ok(settings);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSettings([FromBody] UserSettingsDto updated)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var settings = await _context.UserSettings.FirstOrDefaultAsync(s => s.UserId == userId);
            if (settings == null)
                return NotFound();

            settings.DailyWordLimit = updated.DailyWordLimit;
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
