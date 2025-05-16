using Microsoft.EntityFrameworkCore;
using WordMemoryApi.Data;
using WordMemoryApi.DTOs;
using WordMemoryApi.Entities;

namespace WordMemoryApi.Services
{
    public class UserWordService
    {
        private readonly AppDbContext _context;

        public UserWordService(AppDbContext context)
        {
            _context = context;
        }

        // Kullanıcının tüm UserWord verilerini getir (DTO ile döner)
        public async Task<List<UserWordDto>> GetByUserIdAsync(int userId)
        {
            var userWords = await _context.UserWords
                .Include(uw => uw.Word)
                .Where(uw => uw.UserId == userId)
                .ToListAsync();

            return userWords.Select(uw => new UserWordDto
            {
                Id = uw.Id,
                RepetitionCount = uw.RepetitionCount,
                NextRepetitionDate = uw.NextRepetitionDate,
                IsMastered = uw.IsMastered,
                Word = new WordDto
                {
                    Id = uw.Word.Id,
                    EngWord = uw.Word.EngWord,
                    TrWord = uw.Word.TrWord,
                    Category = uw.Word.Category,
                    SampleSentence = uw.Word.SampleSentence,
                    AudioPath = uw.Word.AudioPath,
                    ImagePath = uw.Word.ImagePath
                }
            }).ToList();
        }

        // Belirli bir kullanıcı için bir kelimeyle ilişki var mı?
        public async Task<UserWord?> GetByUserAndWordAsync(int userId, int wordId)
        {
            return await _context.UserWords
                .FirstOrDefaultAsync(uw => uw.UserId == userId && uw.WordId == wordId);
        }

        // Yeni ilişki oluştur
        public async Task<UserWord> CreateAsync(int userId, int wordId)
        {
            var wordExists = await _context.Words.AnyAsync(w => w.Id == wordId);
            if (!wordExists)
                throw new Exception("WordId geçersiz.");

            var userWord = new UserWord
            {
                UserId = userId,
                WordId = wordId,
                RepetitionCount = 0,
                NextRepetitionDate = DateTime.UtcNow.AddDays(1)
            };

            _context.UserWords.Add(userWord);
            await _context.SaveChangesAsync();

            return userWord;
        }

        // Tekrar sonrası güncelle
        public async Task<bool> IncrementRepetitionAsync(int userId, int wordId)
        {
            var userWord = await GetByUserAndWordAsync(userId, wordId);
            if (userWord == null) return false;

            if (userWord.RepetitionCount >= 6)
            {
                userWord.IsMastered = true;
                return false; // zaten öğrenilmiş
            }

            userWord.RepetitionCount += 1;
            userWord.NextRepetitionDate = GetNextDate(userWord.RepetitionCount);

            if (userWord.RepetitionCount >= 6)
            {
                userWord.IsMastered = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private DateTime GetNextDate(int repetition)
        {
            return repetition switch
            {
                1 => DateTime.UtcNow.AddDays(1),
                2 => DateTime.UtcNow.AddDays(7),
                3 => DateTime.UtcNow.AddMonths(1),
                4 => DateTime.UtcNow.AddMonths(3),
                5 => DateTime.UtcNow.AddMonths(6),
                6 => DateTime.UtcNow.AddYears(1),
                _ => DateTime.UtcNow
            };
        }

        public async Task AddDailyWordsAsync(User user)
        {
            var settings = await _context.UserSettings.FirstOrDefaultAsync(s => s.UserId == user.Id);
            if (settings == null) return;

            if (settings.LastWordAdditionDate.Date >= DateTime.Today)
                return;

            var learnedWordIds = await _context.UserWords
                .Where(uw => uw.UserId == user.Id)
                .Select(uw => uw.WordId)
                .ToListAsync();

            var newWords = await _context.Words
                .Where(w => !learnedWordIds.Contains(w.Id))
                .OrderBy(w => Guid.NewGuid())
                .Take(settings.DailyWordLimit)
                .ToListAsync();

            foreach (var word in newWords)
            {
                _context.UserWords.Add(new UserWord
                {
                    UserId = user.Id,
                    WordId = word.Id,
                    RepetitionCount = 0,
                    NextRepetitionDate = DateTime.UtcNow.AddDays(1),
                    IsMastered = false,
                    AddedDate = DateTime.UtcNow
                });
            }

            settings.LastWordAdditionDate = DateTime.Today;
            await _context.SaveChangesAsync();
        }

    }
}
