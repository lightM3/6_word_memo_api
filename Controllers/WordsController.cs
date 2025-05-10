using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WordMemoryApi.Data;
using WordMemoryApi.DTOs;
using WordMemoryApi.Entities;
using WordMemoryApi.Services;

namespace WordMemoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WordsController : ControllerBase
    {
        private readonly WordService _wordService;
        private readonly AppDbContext _context;

        public WordsController(WordService wordService, AppDbContext context)
        {
            _wordService = wordService;
            _context = context;
        }


        // Yeni kelime ekle
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] WordDto dto)
        {
            // 1. Aynı İngilizce kelime veritabanında zaten varsa, ekleme
            var exists = await _context.Words.AnyAsync(w => w.EngWord.ToLower() == dto.EngWord.ToLower());
            if (exists)
                return BadRequest("Bu kelime zaten mevcut.");

            // 2. Kelimeyi veritabanına ekle
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var word = await _wordService.AddAsync(dto);

            // 3. Kullanıcı ile ilişkilendir
            var userWord = new UserWord
            {
                UserId = userId,
                WordId = word.Id,
                RepetitionCount = 0,
                NextRepetitionDate = DateTime.UtcNow.AddDays(1)
            };

            _context.UserWords.Add(userWord);
            await _context.SaveChangesAsync();

            return Ok(word);
        }




        // Var olan kelimeyi güncelle
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] WordDto dto)
        {
            var success = await _wordService.UpdateAsync(id, dto);
            if (!success) return NotFound();
            return NoContent();
        }

        // Kelimeyi sil
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _wordService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        //Fotoğraf ekle
        [HttpPost("upload-image")]
        [Authorize]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Dosya bulunamadı");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = $"images/{fileName}";
            return Ok(new { imagePath = relativePath });
        }
        // Ses dosyası ekleme
        [HttpPost("upload-audio")]
        [Authorize]
        public async Task<IActionResult> UploadAudio(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Dosya bulunamadı");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "audio");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = $"audio/{fileName}";
            return Ok(new { audioPath = relativePath });
        }

        [HttpGet("all")]
        [Authorize]
        public async Task<IActionResult> GetAllWords()
        {
            var words = await _context.Words.ToListAsync();
            return Ok(words);
        }


    }


}
