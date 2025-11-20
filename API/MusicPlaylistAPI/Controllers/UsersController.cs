using Microsoft.AspNetCore.Mvc;
using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Models.Dto.Get;
using MusicPlaylistAPI.Services.Interface;

namespace MusicPlaylistAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync() =>
            Ok(await _userService.GetAsync());

        [HttpGet("{id}", Name = "GetUserById")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            try
            {
                return Ok(await _userService.GetAsync(id));
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }

        [HttpGet("email")]
        public async Task<IActionResult> GetByEmailAsync([FromQuery] string email)
        {
            try
            {
                return Ok(await _userService.GetByEmailAsync(email));
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(UserCreateDto user)
        {
            UserGetDto userGet = await _userService.CreateAsync(user);
            return CreatedAtRoute("GetUserById", new { id = userGet.Id }, userGet);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(string id, UserCreateDto user)
        {
            try
            {
                UserGetDto userGet = await _userService.UpdateAsync(id, user);
                return Ok(userGet);
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
                await _userService.DeleteAsync(id);
                return NoContent();
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }
    }
}
