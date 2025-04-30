using Microsoft.EntityFrameworkCore;
using WordMemoryApi.Data;
using WordMemoryApi.DTOs;
using WordMemoryApi.Entities;

namespace WordMemoryApi.Services
{
    public class WordService
    {
        private readonly AppDbContext _context;

        public WordService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Word>> GetAllAsync()
        {
            return await _context.Words.ToListAsync();
        }

        public async Task<Word?> GetByIdAsync(int id)
        {
            return await _context.Words.FindAsync(id);
        }

        public async Task<Word> AddAsync(WordDto dto)
        {
            var word = new Word
            {
                EngWord = dto.EngWord,
                TrWord = dto.TrWord,
                SampleSentence = dto.SampleSentence,
                ImagePath = dto.ImagePath,
                Category = dto.Category,
                AddedDate = DateTime.Now
            };

            _context.Words.Add(word);
            await _context.SaveChangesAsync();
            return word;
        }

        public async Task<bool> UpdateAsync(int id, WordDto dto)
        {
            var word = await _context.Words.FindAsync(id);
            if (word == null) return false;

            word.EngWord = dto.EngWord;
            word.TrWord = dto.TrWord;
            word.SampleSentence = dto.SampleSentence;
            word.ImagePath = dto.ImagePath;
            word.Category = dto.Category;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var word = await _context.Words.FindAsync(id);
            if (word == null) return false;

            _context.Words.Remove(word);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
