using Microsoft.AspNetCore.Mvc;
using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Models.Dto.Get;
using MusicPlaylistAPI.Services.Interface;

namespace MusicPlaylistAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                return Ok(await _commentService.GetAsync());
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            try
            {
                return Ok(await _commentService.GetAsync(id));
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
        public async Task<IActionResult> GetByPlaylistidAsync(string id)
        {
            try
            {
                return Ok(await _commentService.GetByPlaylistAsync(id));
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(CommentCreateDto comment)
        {
            CommentGetDto commentGet = await _commentService.CreateAsync(comment);
            return CreatedAtAction(nameof(GetByIdAsync), commentGet);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            try
            {
                await _commentService.DeleteAsync(id);
                return NoContent();
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }
    }
}
