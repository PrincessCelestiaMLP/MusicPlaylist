using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MusicPlaylistAPI.Controllers;
using MusicPlaylistAPI.Models.Dto;
using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Models.Dto.Get;
using MusicPlaylistAPI.Services.Interface;
using Xunit;

namespace MusicPlaylistAPI.Tests
{
    public class PlaylistsControllerTests
    {
        private readonly Mock<IPlaylistService> _mockService;
        private readonly PlaylistsController _controller;

        public PlaylistsControllerTests()
        {
            _mockService = new Mock<IPlaylistService>();
            _controller = new PlaylistsController(_mockService.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkResult_WithListOfPlaylists()
        {
            // Arrange
            var playlists = new List<PlaylistGetDto>
            {
                new PlaylistGetDto { Id = "1", Title = "Playlist1", CreatedAt = DateTime.UtcNow },
                new PlaylistGetDto { Id = "2", Title = "Playlist2", CreatedAt = DateTime.UtcNow }
            };
            _mockService.Setup(s => s.GetAsync()).ReturnsAsync(playlists);

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(playlists, okResult.Value);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsOkResult_WhenFound()
        {
            var playlist = new PlaylistGetDto { Id = "1", Title = "Playlist1", CreatedAt = DateTime.UtcNow };
            _mockService.Setup(s => s.GetAsync("1")).ReturnsAsync(playlist);

            var result = await _controller.GetByIdAsync("1");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(playlist, okResult.Value);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNotFound_WhenNullReference()
        {
            _mockService.Setup(s => s.GetAsync("1")).ThrowsAsync(new NullReferenceException());

            var result = await _controller.GetByIdAsync("1");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetByUserIdAsync_ReturnsOkResult_WithListOfPlaylists()
        {
            var playlists = new List<PlaylistGetDto>
            {
                new PlaylistGetDto { Id = "1", Title = "Playlist1", CreatedAt = DateTime.UtcNow }
            };
            _mockService.Setup(s => s.GetByUserAsync("user1")).ReturnsAsync(playlists);

            var result = await _controller.GetByUserIdAsync("user1");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(playlists, okResult.Value);
        }

        [Fact]
        public async Task PostAsync_ReturnsCreatedAtActionResult()
        {
            var playlistCreate = new PlaylistCreateDto
            {
                Title = "New Playlist",
                Cover = new byte[] { 1, 2, 3 },
                UserId = "user1"
            };
            var playlistGet = new PlaylistGetDto
            {
                Id = "1",
                Title = "New Playlist",
                Cover = new byte[] { 1, 2, 3 },
                CreatedAt = DateTime.UtcNow
            };
            _mockService.Setup(s => s.CreateAsync(playlistCreate)).ReturnsAsync(playlistGet);

            var result = await _controller.PostAsync(playlistCreate);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(playlistGet, createdResult.Value);
        }

        [Fact]
        public async Task PutAsync_ReturnsOkResult_WhenSuccess()
        {
            var playlistUpdate = new PlaylistCreateDto
            {
                Title = "Updated Playlist",
                Cover = new byte[] { 4, 5, 6 },
                UserId = "user1"
            };
            var playlistGet = new PlaylistGetDto
            {
                Id = "1",
                Title = "Updated Playlist",
                Cover = new byte[] { 4, 5, 6 },
                CreatedAt = DateTime.UtcNow
            };
            _mockService.Setup(s => s.UpdateAsync("1", playlistUpdate)).ReturnsAsync(playlistGet);

            var result = await _controller.PutAsync("1", playlistUpdate);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(playlistGet, okResult.Value);
        }

        [Fact]
        public async Task PutAsync_ReturnsNotFound_WhenNullReference()
        {
            var playlistUpdate = new PlaylistCreateDto
            {
                Title = "Updated Playlist",
                Cover = new byte[] { 4, 5, 6 },
                UserId = "user1"
            };
            _mockService.Setup(s => s.UpdateAsync("1", playlistUpdate)).ThrowsAsync(new NullReferenceException());

            var result = await _controller.PutAsync("1", playlistUpdate);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNoContent_WhenSuccess()
        {
            _mockService.Setup(s => s.DeleteAsync("1")).Returns(Task.CompletedTask);

            var result = await _controller.DeleteAsync("1");

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound_WhenThrowsNullReference()
        {
            _mockService.Setup(s => s.DeleteAsync("1")).ThrowsAsync(new NullReferenceException());

            var result = await _controller.DeleteAsync("1");

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
