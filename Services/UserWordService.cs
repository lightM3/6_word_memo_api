using Microsoft.EntityFrameworkCore;
using WordMemoryApi.Data;
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

        // Kullanıcının tüm UserWord verilerini getir
        public async Task<List<UserWord>> GetByUserIdAsync(int userId)
        {
            return await _context.UserWords
                .Include(uw => uw.Word)
                .Where(uw => uw.UserId == userId)
                .ToListAsync();
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
            var userWord = new UserWord
            {
                UserId = userId,
                WordId = wordId,
                RepetitionCount = 0,
                NextRepetitionDate = DateTime.UtcNow.AddDays(1)
            };
            var wordExists = await _context.Words.AnyAsync(w => w.Id == wordId);
            if (!wordExists)
                throw new Exception("WordId geçersiz.");

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
    }
}
