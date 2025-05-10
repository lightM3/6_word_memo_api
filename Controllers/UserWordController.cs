using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WordMemoryApi.Data;
using WordMemoryApi.Services;

namespace WordMemoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserWordController : ControllerBase
    {
        private readonly UserWordService _userWordService;
        private readonly AppDbContext _context;

        public UserWordController(UserWordService userWordService, AppDbContext context)
        {
            _userWordService = userWordService;
            _context = context;
        }


        // Kullanıcının tekrar listesi
        [HttpGet("due")]
        public async Task<IActionResult> GetDueWords()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var allWords = await _userWordService.GetByUserIdAsync(userId);
            var dueWords = allWords.Where(uw =>
                uw.NextRepetitionDate.HasValue &&
                uw.NextRepetitionDate.Value.Date <= DateTime.UtcNow.Date &&
                !uw.IsMastered).ToList();

            return Ok(dueWords);
        }

        // Kullanıcı kelimeyi ilk defa çalışıyorsa ilişki oluştur
        [HttpPost("{wordId}")]
        public async Task<IActionResult> StartLearning(int wordId)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var existing = await _userWordService.GetByUserAndWordAsync(userId, wordId);
            if (existing != null)
                return BadRequest("Zaten eklenmiş");

            var result = await _userWordService.CreateAsync(userId, wordId);
            return Ok(result);
        }

        // Doğru bilinen kelimenin tekrar sayısını artır
        [HttpPost("{wordId}/repeat")]
        public async Task<IActionResult> RepeatSuccess(int wordId)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var success = await _userWordService.IncrementRepetitionAsync(userId, wordId);
            if (!success) return BadRequest("Tekrar artırılamadı veya zaten tamamlandı");

            return Ok("Tekrar sayısı artırıldı");
        }


        //Kullanıcının eklediği kelimeleri listeleme
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUserWords()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var allWords = await _userWordService.GetByUserIdAsync(userId);
            return Ok(allWords);
        }

        [HttpGet("analysis")]
        [Authorize]
        public async Task<IActionResult> GetAnalysis()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var userWords = await _context.UserWords
                .Include(uw => uw.Word)
                .Where(uw => uw.UserId == userId)
                .ToListAsync();

            int total = userWords.Count;
            int learned = userWords.Count(w => w.RepetitionCount >= 6);

            var categoryMap = userWords
                .GroupBy(w => w.Word.Category ?? "belirsiz")
                .ToDictionary(
                    g => g.Key,
                    g => g.Count(w => w.RepetitionCount >= 6)
                );

            return Ok(new
            {
                total,
                learned,
                successRate = total == 0 ? 0 : (double)learned / total * 100,
                categorySuccess = categoryMap
            });
        }


        [HttpGet("category-stats")]
        public async Task<IActionResult> GetCategoryStats()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var userWords = await _userWordService.GetByUserIdAsync(userId);

            var grouped = userWords
                .GroupBy(w => w.Word.Category ?? "belirsiz")
                .Select(g => new
                {
                    Category = g.Key,
                    LearnedCount = g.Count(w => w.RepetitionCount >= 6)
                });

            return Ok(grouped);
        }



    }
}
