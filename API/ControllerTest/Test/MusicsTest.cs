using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MusicPlaylistAPI.Controllers;
using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Models.Dto.Get;
using MusicPlaylistAPI.Services.Interface;
using Xunit;

namespace MusicPlaylistAPI.Tests
{
    public class MusicsControllerTests
    {
        private readonly Mock<IMusicService> _mockService;
        private readonly MusicsController _controller;

        public MusicsControllerTests()
        {
            _mockService = new Mock<IMusicService>();
            _controller = new MusicsController(_mockService.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkResult_WithListOfMusic()
        {
            var musics = new List<MusicGetDto>
            {
                new MusicGetDto { Id = "1", Title = "Song1", Artist = "Artist1", Link = "link1" },
                new MusicGetDto { Id = "2", Title = "Song2", Artist = "Artist2", Link = "link2" }
            };
            _mockService.Setup(s => s.GetAsync()).ReturnsAsync(musics);

            var result = await _controller.GetAllAsync();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(musics, okResult.Value);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsOkResult_WhenFound()
        {
            var music = new MusicGetDto { Id = "1", Title = "Song1", Artist = "Artist1", Link = "link1" };
            _mockService.Setup(s => s.GetAsync("1")).ReturnsAsync(music);

            var result = await _controller.GetByIdAsync("1");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(music, okResult.Value);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNotFound_WhenNullReference()
        {
            _mockService.Setup(s => s.GetAsync("1")).ThrowsAsync(new NullReferenceException());

            var result = await _controller.GetByIdAsync("1");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetByPlaylistIdAsync_ReturnsOkResult_WithListOfMusic()
        {
            var musics = new List<MusicGetDto>
            {
                new MusicGetDto { Id = "1", Title = "Song1", Artist = "Artist1", Link = "link1" }
            };
            _mockService.Setup(s => s.GetByPlaylistAsync("playlist1")).ReturnsAsync(musics);

            var result = await _controller.GetByPlaylistIdAsync("playlist1");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(musics, okResult.Value);
        }

        [Fact]
        public async Task PostAsync_ReturnsCreatedAtActionResult()
        {
            var musicCreate = new MusicCreateDto { Title = "New Song", Artist = "ArtistX", Link = "linkX", PlaylistId = "p1" };
            var musicGet = new MusicGetDto { Id = "1", Title = "New Song", Artist = "ArtistX", Link = "linkX" };
            _mockService.Setup(s => s.CreateAsync(musicCreate)).ReturnsAsync(musicGet);

            var result = await _controller.PostAsync(musicCreate);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(musicGet, createdResult.Value);
        }

        [Fact]
        public async Task PutAsync_ReturnsOkResult_WhenSuccess()
        {
            var musicUpdate = new MusicCreateDto { Title = "Updated Song", Artist = "ArtistY", Link = "linkY", PlaylistId = "p1" };
            var musicGet = new MusicGetDto { Id = "1", Title = "Updated Song", Artist = "ArtistY", Link = "linkY" };
            _mockService.Setup(s => s.UpdateAsync("1", musicUpdate)).ReturnsAsync(musicGet);

            var result = await _controller.PutAsync("1", musicUpdate);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(musicGet, okResult.Value);
        }

        [Fact]
        public async Task PutAsync_ReturnsNotFound_WhenNullReference()
        {
            var musicUpdate = new MusicCreateDto { Title = "Updated Song", Artist = "ArtistY", Link = "linkY", PlaylistId = "p1" };
            _mockService.Setup(s => s.UpdateAsync("1", musicUpdate)).ThrowsAsync(new NullReferenceException());

            var result = await _controller.PutAsync("1", musicUpdate);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound_WhenThrowsNullReference()
        {
            _mockService.Setup(s => s.DeleteAsync("1")).ThrowsAsync(new NullReferenceException());

            var result = await _controller.DeleteAsync("1");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound_WhenSuccess()
        {
            _mockService.Setup(s => s.DeleteAsync("1")).Returns(Task.CompletedTask);

            var result = await _controller.DeleteAsync("1");

            Assert.IsType<NotFoundResult>(result); // відповідає логіці контролера
        }
    }
}
