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
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _mockService;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockService = new Mock<IUserService>();
            _controller = new UsersController(_mockService.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkResult_WithListOfUsers()
        {
            var users = new List<UserGetDto>
            {
                new UserGetDto { Id = "1", Username = "User1", Email = "user1@test.com" },
                new UserGetDto { Id = "2", Username = "User2", Email = "user2@test.com" }
            };
            _mockService.Setup(s => s.GetAsync()).ReturnsAsync(users);

            var result = await _controller.GetAllAsync();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(users, okResult.Value);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsOkResult_WhenFound()
        {
            var user = new UserGetDto { Id = "1", Username = "User1", Email = "user1@test.com" };
            _mockService.Setup(s => s.GetAsync("1")).ReturnsAsync(user);

            var result = await _controller.GetByIdAsync("1");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(user, okResult.Value);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNotFound_WhenNullReference()
        {
            _mockService.Setup(s => s.GetAsync("1")).ThrowsAsync(new NullReferenceException());

            var result = await _controller.GetByIdAsync("1");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetByEmailAsync_ReturnsOkResult_WhenFound()
        {
            var user = new UserGetDto { Id = "1", Username = "User1", Email = "user1@test.com" };
            _mockService.Setup(s => s.GetByEmailAsync("user1@test.com")).ReturnsAsync(user);

            var result = await _controller.GetByEmailAsync("user1@test.com");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(user, okResult.Value);
        }

        [Fact]
        public async Task GetByEmailAsync_ReturnsNotFound_WhenNullReference()
        {
            _mockService.Setup(s => s.GetByEmailAsync("user1@test.com")).ThrowsAsync(new NullReferenceException());

            var result = await _controller.GetByEmailAsync("user1@test.com");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PostAsync_ReturnsCreatedAtActionResult()
        {
            var userCreate = new UserCreateDto
            {
                Username = "NewUser",
                Email = "newuser@test.com",
                Password = "password123"
            };
            var userGet = new UserGetDto
            {
                Id = "1",
                Username = "NewUser",
                Email = "newuser@test.com"
            };
            _mockService.Setup(s => s.CreateAsync(userCreate)).ReturnsAsync(userGet);

            var result = await _controller.PostAsync(userCreate);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(userGet, createdResult.Value);
        }

        [Fact]
        public async Task PutAsync_ReturnsOkResult_WhenSuccess()
        {
            var userUpdate = new UserCreateDto
            {
                Username = "UpdatedUser",
                Email = "updated@test.com",
                Password = "newpass123"
            };
            var userGet = new UserGetDto
            {
                Id = "1",
                Username = "UpdatedUser",
                Email = "updated@test.com"
            };
            _mockService.Setup(s => s.UpdateAsync("1", userUpdate)).ReturnsAsync(userGet);

            var result = await _controller.PutAsync("1", userUpdate);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(userGet, okResult.Value);
        }

        [Fact]
        public async Task PutAsync_ReturnsNotFound_WhenNullReference()
        {
            var userUpdate = new UserCreateDto
            {
                Username = "UpdatedUser",
                Email = "updated@test.com",
                Password = "newpass123"
            };
            _mockService.Setup(s => s.UpdateAsync("1", userUpdate)).ThrowsAsync(new NullReferenceException());

            var result = await _controller.PutAsync("1", userUpdate);

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
