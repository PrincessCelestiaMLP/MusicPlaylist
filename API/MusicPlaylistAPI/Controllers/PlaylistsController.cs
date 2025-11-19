using Microsoft.AspNetCore.Mvc;
using MusicPlaylistAPI.Models.Dto;
using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Services.Interface;

namespace MusicPlaylistAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistsController : ControllerBase
    {
        private readonly IPlaylistService _playlistService;

        public PlaylistsController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync() =>
            Ok(await _playlistService.GetAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            try
            {
                return Ok(await _playlistService.GetAsync(id));
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }

        [HttpGet("Users/{id}")]
        public async Task<IActionResult> GetByUserIdAsync(string id) =>
            Ok(await _playlistService.GetByUserAsync(id));

        [HttpPost]
        public async Task<IActionResult> PostAsync(PlaylistCreateDto playlist)
        {
            PlaylistGetDto playlistGet = await _playlistService.CreateAsync(playlist);
            return CreatedAtAction(nameof(GetByIdAsync), playlistGet);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(string id, PlaylistCreateDto playlist)
        {
            try
            {
                PlaylistGetDto playlistGet = await _playlistService.UpdateAsync(id, playlist);
                return Ok(playlistGet);
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
                await _playlistService.DeleteAsync(id);
                return NoContent();
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }
    }
}
