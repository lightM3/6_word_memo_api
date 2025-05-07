using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WordMemoryApi.Services;
using WordMemoryApi.Entities;

namespace WordMemoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuizController : ControllerBase
    {
        private readonly UserWordService _userWordService;

        public QuizController(UserWordService userWordService)
        {
            _userWordService = userWordService;
        }

        // 1. Quiz sorularını getir (bugün tekrar edilmesi gerekenler)
        [HttpGet("today")]
        public async Task<IActionResult> GetTodayQuiz()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userWords = await _userWordService.GetByUserIdAsync(userId);
            var dueWords = userWords
                .Where(uw => uw.NextRepetitionDate <= DateTime.UtcNow && !uw.IsMastered)
                .Select(uw => new
                {
                    WordId = uw.WordId,
                    EngWord = uw.Word.EngWord,
                    TrWord = uw.Word.TrWord,
                    SampleSentence = uw.Word.SampleSentence
                }).ToList();

            return Ok(dueWords);
        }

        // 2. Quiz sonucu gönder (doğru bilinen wordId listesi)
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitQuiz([FromBody] List<int> correctWordIds)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            int updated = 0;
            foreach (var wordId in correctWordIds)
            {
                var result = await _userWordService.IncrementRepetitionAsync(userId, wordId);
                if (result) updated++;
            }

            return Ok(new
            {
                Message = "Quiz başarıyla kaydedildi.",
                CorrectUpdatedCount = updated
            });
        }
    }
}
