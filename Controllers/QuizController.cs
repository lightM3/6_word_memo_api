using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WordMemoryApi.Services;
using WordMemoryApi.Entities;
using WordMemoryApi.Data;
using Microsoft.EntityFrameworkCore;

namespace WordMemoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuizController : ControllerBase
    {
        private readonly UserWordService _userWordService;
        private readonly AppDbContext _context;

        public QuizController(UserWordService userWordService, AppDbContext context)
        {
            _userWordService = userWordService;
            _context = context;
        }


        // 1. Quiz sorularını getir (bugün tekrar edilmesi gerekenler)
        [HttpGet("today")]
        public async Task<IActionResult> GetTodayQuiz()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Kullanıcının ilişkili kelimelerini getir
            var userWords = await _userWordService.GetByUserIdAsync(userId);

            // Tekrar zamanı gelen kelimeleri bul
            var dueWords = userWords
                .Where(uw => uw.NextRepetitionDate <= DateTime.UtcNow && !uw.IsMastered)
                .Select(uw => new
                {
                    WordId = uw.Word.Id,
                    EngWord = uw.Word.EngWord,
                    TrWord = uw.Word.TrWord,
                    SampleSentence = uw.Word.SampleSentence,
                    RepetitionCount = uw.RepetitionCount
                }).ToList();

            // Eğer kullanıcı yeni ve hiç ilişki yoksa: havuzdan kelime ver
            if (dueWords.Count == 0)
            {
                if (userWords.Count == 0)
                {
                    // İlk kez kullanıcı – havuzdan kelime çek
                    var randomWords = await _context.Words
                        .OrderBy(w => Guid.NewGuid())
                        .Take(10)
                        .ToListAsync();

                    var response = randomWords.Select(w => new
                    {
                        WordId = w.Id,
                        EngWord = w.EngWord,
                        TrWord = w.TrWord,
                        SampleSentence = w.SampleSentence,
                        RepetitionCount = 0
                    });

                    return Ok(response);
                }
                else
                {
                    // UserWord var ama bugünlük tekrar yok
                    return Ok(new List<object>());
                }
            }

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
                // Eğer UserWord ilişkisi yoksa oluştur, varsa güncelle
                var existing = await _userWordService.GetByUserAndWordAsync(userId, wordId);

                if (existing == null)
                {
                    await _userWordService.CreateAsync(userId, wordId);
                    updated++;
                }
                else
                {
                    var result = await _userWordService.IncrementRepetitionAsync(userId, wordId);
                    if (result) updated++;
                }
            }


            return Ok(new
            {
                Message = "Quiz başarıyla kaydedildi.",
                CorrectUpdatedCount = updated
            });
        }
    }
}
