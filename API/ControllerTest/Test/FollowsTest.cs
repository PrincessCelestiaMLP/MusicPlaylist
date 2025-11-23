using Microsoft.AspNetCore.Mvc;
using Moq;
using MusicPlaylistAPI.Controllers;
using MusicPlaylistAPI.Models;
using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Models.Dto.Get;
using MusicPlaylistAPI.Services.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MusicPlaylistAPI.Tests
{
    public class FollowsControllerTests
    {
        private readonly Mock<IFollowService> _mockService;
        private readonly FollowsController _controller;

        public FollowsControllerTests()
        {
            _mockService = new Mock<IFollowService>();
            _controller = new FollowsController(_mockService.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkResult_WithListOfFollows()
        {
            // Arrange
            var follows = new List<FollowGetDto>
            {
                new FollowGetDto { Id = "1", Follower = new UserView { Id = "u1", Username = "User1" }, FollowedAt = DateTime.UtcNow },
                new FollowGetDto { Id = "2", Follower = new UserView { Id = "u2", Username = "User2" }, FollowedAt = DateTime.UtcNow }
            };
            _mockService.Setup(s => s.GetAsync()).ReturnsAsync(follows);

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(follows, okResult.Value);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsBadRequest_OnArgumentException()
        {
            _mockService.Setup(s => s.GetAsync()).ThrowsAsync(new ArgumentException("Error"));

            var result = await _controller.GetAllAsync();

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error", badRequestResult.Value);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsOkResult_WhenFound()
        {
            var follow = new FollowGetDto { Id = "1", Follower = new UserView { Id = "u1", Username = "User1" }, FollowedAt = DateTime.UtcNow };
            _mockService.Setup(s => s.GetAsync("1")).ReturnsAsync(follow);

            var result = await _controller.GetByIdAsync("1");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(follow, okResult.Value);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNotFound_WhenNullReference()
        {
            _mockService.Setup(s => s.GetAsync("1")).ThrowsAsync(new NullReferenceException());

            var result = await _controller.GetByIdAsync("1");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsBadRequest_OnArgumentException()
        {
            _mockService.Setup(s => s.GetAsync("1")).ThrowsAsync(new ArgumentException("Error"));

            var result = await _controller.GetByIdAsync("1");

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error", badRequestResult.Value);
        }

        [Fact]
        public async Task PostAsync_ReturnsCreatedAtActionResult()
        {
            var followCreate = new FollowCreteDto { /* заповнити необхідні поля */ };
            var followGet = new FollowGetDto { Id = "1", Follower = new UserView { Id = "u1", Username = "User1" }, FollowedAt = DateTime.UtcNow };
            _mockService.Setup(s => s.CreateAsync(followCreate)).ReturnsAsync(followGet);

            var result = await _controller.PostAsync(followCreate);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(followGet, createdResult.Value);
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

            Assert.IsType<NotFoundResult>(result); // логіка контролера: NotFound навіть при успіху
        }
    }
}
