using Microsoft.AspNetCore.Mvc;
using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Models.Dto.Get;
using MusicPlaylistAPI.Services.Interface;

namespace MusicPlaylistAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicsController : ControllerBase
    {
        private readonly IMusicService _musicService;

        public MusicsController(IMusicService musicService)
        {
            _musicService = musicService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync() =>
            Ok(await _musicService.GetAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            try
            {
                return Ok(await _musicService.GetAsync(id));
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }

        [HttpGet("Playlists/{id}")]
        public async Task<IActionResult> GetByPlaylistIdAsync(string id) =>
            Ok(await _musicService.GetByPlaylistAsync(id));

        [HttpPost]
        public async Task<IActionResult> PostAsync(MusicCreateDto music)
        {
            MusicGetDto musicGet = await _musicService.CreateAsync(music);
            return CreatedAtAction(nameof(GetByIdAsync), musicGet);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(string id, MusicCreateDto music)
        {
            try
            {
                MusicGetDto musicGet = await _musicService.UpdateAsync(id, music);
                return Ok(musicGet);
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            try
            {
                await _musicService.DeleteAsync(id);
                return NotFound();
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }
    }
}
