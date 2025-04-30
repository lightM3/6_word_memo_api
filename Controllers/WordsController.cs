using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WordMemoryApi.DTOs;
using WordMemoryApi.Services;

namespace WordMemoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WordsController : ControllerBase
    {
        private readonly WordService _wordService;

        public WordsController(WordService wordService)
        {
            _wordService = wordService;
        }

        // Tüm kelimeleri getir
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var words = await _wordService.GetAllAsync();
            return Ok(words);
        }

        // ID'ye göre bir kelime getir
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            var word = await _wordService.GetByIdAsync(id);
            if (word == null) return NotFound();
            return Ok(word);
        }

        // Yeni kelime ekle
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] WordDto dto)
        {
            var word = await _wordService.AddAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = word.Id }, word);
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
    }
}
