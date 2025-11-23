using Xunit;
using Moq;
using MusicPlaylistAPI.Controllers;
using MusicPlaylistAPI.Services.Interface;
using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Models.Dto.Get;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControllerTest.Test
{
    public class CommentsTest
    {
        private readonly Mock<ICommentService> _mockService;
        private readonly CommentsController _controller;

        public CommentsTest()
        {
            _mockService = new Mock<ICommentService>();
            _controller = new CommentsController(_mockService.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOk_WithListOfComments()
        {
            // Arrange
            var comments = new List<CommentGetDto>
            {
                new CommentGetDto { Id = "1", Text = "Comment 1" },
                new CommentGetDto { Id = "2", Text = "Comment 2" }
            };
            _mockService.Setup(s => s.GetAsync()).ReturnsAsync(comments);

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<CommentGetDto>>(okResult.Value);
            Assert.Equal(2, ((List<CommentGetDto>)returnValue).Count);
        }

        [Fact]
        public async Task GetAllAsync_WhenArgumentException_ReturnsBadRequest()
        {
            // Arrange
            _mockService.Setup(s => s.GetAsync()).ThrowsAsync(new ArgumentException("Invalid"));

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid", badRequest.Value);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsOk_WhenCommentExists()
        {
            // Arrange
            var comment = new CommentGetDto { Id = "1", Text = "Test" };
            _mockService.Setup(s => s.GetAsync("1")).ReturnsAsync(comment);

            // Act
            var result = await _controller.GetByIdAsync("1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(comment, okResult.Value);
        }

        [Fact]
        public async Task GetByIdAsync_WhenNullReferenceException_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.GetAsync("1")).ThrowsAsync(new NullReferenceException());

            // Act
            var result = await _controller.GetByIdAsync("1");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PostAsync_ReturnsCreatedAtAction()
        {
            // Arrange
            var createDto = new CommentCreateDto { Text = "New Comment" };
            var getDto = new CommentGetDto { Id = "1", Text = "New Comment" };
            _mockService.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(getDto);

            // Act
            var result = await _controller.PostAsync(createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(getDto, createdResult.Value);
        }

        [Fact]
        public async Task DeleteAsync_WhenCommentExists_ReturnsNoContent()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteAsync("1")).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteAsync("1");

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_WhenNullReferenceException_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteAsync("1")).ThrowsAsync(new NullReferenceException());

            // Act
            var result = await _controller.DeleteAsync("1");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}