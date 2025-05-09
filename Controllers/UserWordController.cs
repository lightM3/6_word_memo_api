using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WordMemoryApi.Services;

namespace WordMemoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserWordController : ControllerBase
    {
        private readonly UserWordService _userWordService;

        public UserWordController(UserWordService userWordService)
        {
            _userWordService = userWordService;
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

    }
}
