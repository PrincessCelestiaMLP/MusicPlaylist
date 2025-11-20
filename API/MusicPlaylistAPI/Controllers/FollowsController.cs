using Microsoft.AspNetCore.Mvc;
using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Models.Dto.Get;
using MusicPlaylistAPI.Services.Interface;

namespace MusicPlaylistAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowsController : ControllerBase
    {
        private readonly IFollowService _followService;

        public FollowsController(IFollowService followService)
        {
            _followService = followService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                return Ok(await _followService.GetAsync());
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}", Name = "GetFollowById")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            try
            {
                return Ok(await _followService.GetAsync(id));
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("Playlists/{id}")]
        public async Task<IActionResult> GetByPlaylistIdAsync(string id)
        {
            try
            {
                return Ok(await _followService.GetByPlaylistAsync(id));
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(FollowCreteDto follow)
        {
            FollowGetDto followGet = await _followService.CreateAsync(follow);
            return CreatedAtRoute("GetFollowById", new { id = followGet.Id }, followGet);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            try
            {
                await _followService.DeleteAsync(id);
                return NotFound();
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }
    }
}
