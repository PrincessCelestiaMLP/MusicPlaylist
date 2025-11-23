using AutoMapper;
using Moq;
using MusicPlaylistAPI.Models.Dto;
using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Models.Dto.Get;
using MusicPlaylistAPI.Models.Entity;
using MusicPlaylistAPI.Repositories.Interface;
using MusicPlaylistAPI.Services;
using MusicPlaylistAPI.Services.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MusicPlaylistAPI.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IPlaylistService> _mockPlaylistService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockPlaylistService = new Mock<IPlaylistService>();
            _mockMapper = new Mock<IMapper>();
            _service = new UserService(_mockUserRepo.Object, _mockPlaylistService.Object, _mockMapper.Object);
        }

        [Fact]
        // Перевіряє створення користувача та повернення UserGetDto
        public async Task CreateAsync_ShouldReturnUserGetDto()
        {
            var userCreate = new UserCreateDto { Username = "User1", Email = "user1@test.com", Password = "pass" };
            var userEntity = new User { Id = "u1", Username = "User1", Email = "user1@test.com", Password = "pass" };
            var userGet = new UserGetDto { Id = "u1", Username = "User1", Email = "user1@test.com", Password = "pass" };

            _mockMapper.Setup(m => m.Map<User>(userCreate)).Returns(userEntity);
            _mockUserRepo.Setup(r => r.CreateAsync(userEntity)).Returns(Task.CompletedTask);
            _mockUserRepo.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(userEntity);
            _mockMapper.Setup(m => m.Map<UserGetDto>(userEntity)).Returns(userGet);
            _mockPlaylistService.Setup(s => s.GetByUserAsync("u1")).ReturnsAsync(new List<PlaylistGetDto>());

            var result = await _service.CreateAsync(userCreate);

            Assert.NotNull(result);
            Assert.Equal("u1", result.Id);
            Assert.Equal("User1", result.Username);
        }

        [Fact]
        // Перевіряє отримання всіх користувачів
        public async Task GetAsync_ShouldReturnListOfUsers()
        {
            var users = new List<User> { new User { Id = "u1", Username = "User1", Email = "user1@test.com", Password = "pass" } };
            var usersGet = new List<UserGetDto> { new UserGetDto { Id = "u1", Username = "User1", Email = "user1@test.com", Password = "pass" } };

            _mockUserRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(users);
            _mockMapper.Setup(m => m.Map<List<UserGetDto>>(users)).Returns(usersGet);
            _mockPlaylistService.Setup(s => s.GetByUserAsync("u1")).ReturnsAsync(new List<PlaylistGetDto>());

            var result = await _service.GetAsync();

            Assert.Single(result);
            Assert.Equal("u1", result[0].Id);
            Assert.NotNull(result[0].Playlists);
        }

        [Fact]
        // Перевіряє отримання користувача за Id
        public async Task GetAsync_ById_ShouldReturnUser()
        {
            var userEntity = new User { Id = "u1", Username = "User1", Email = "user1@test.com", Password = "pass" };
            var userGet = new UserGetDto { Id = "u1", Username = "User1", Email = "user1@test.com", Password = "pass" };

            _mockUserRepo.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(userEntity);
            _mockMapper.Setup(m => m.Map<UserGetDto>(userEntity)).Returns(userGet);
            _mockPlaylistService.Setup(s => s.GetByUserAsync("u1")).ReturnsAsync(new List<PlaylistGetDto>());

            var result = await _service.GetAsync("u1");

            Assert.Equal("u1", result.Id);
        }

        [Fact]
        // Перевіряє отримання користувача за Email
        public async Task GetByEmailAsync_ShouldReturnUser()
        {
            var userEntity = new User { Id = "u1", Username = "User1", Email = "user1@test.com", Password = "pass" };
            var userGet = new UserGetDto { Id = "u1", Username = "User1", Email = "user1@test.com", Password = "pass" };

            _mockUserRepo.Setup(r => r.GetByEmailAsync("user1@test.com")).ReturnsAsync(userEntity);
            _mockMapper.Setup(m => m.Map<UserGetDto>(userEntity)).Returns(userGet);
            _mockPlaylistService.Setup(s => s.GetByUserAsync("u1")).ReturnsAsync(new List<PlaylistGetDto>());

            var result = await _service.GetByEmailAsync("user1@test.com");

            Assert.Equal("u1", result.Id);
            Assert.Equal("User1", result.Username);
        }

        [Fact]
        // Перевіряє оновлення користувача
        public async Task UpdateAsync_ShouldReturnUpdatedUser()
        {
            var userCreate = new UserCreateDto { Username = "UpdatedUser", Email = "updated@test.com", Password = "pass" };
            var userEntity = new User { Id = "u1", Username = "UpdatedUser", Email = "updated@test.com", Password = "pass" };
            var userGet = new UserGetDto { Id = "u1", Username = "UpdatedUser", Email = "updated@test.com", Password = "pass" };

            _mockUserRepo.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(userEntity);
            _mockMapper.Setup(m => m.Map<User>(userCreate)).Returns(userEntity);
            _mockUserRepo.Setup(r => r.UpdateAsync("u1", userEntity)).Returns(Task.CompletedTask);
            _mockUserRepo.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(userEntity);
            _mockMapper.Setup(m => m.Map<UserGetDto>(userEntity)).Returns(userGet);

            var result = await _service.UpdateAsync("u1", userCreate);

            Assert.Equal("u1", result.Id);
            Assert.Equal("UpdatedUser", result.Username);
        }

        [Fact]
        // Перевіряє видалення користувача разом з його плейлистами
        public async Task DeleteAsync_ShouldDeleteUserAndPlaylists()
        {
            var userEntity = new User { Id = "u1", Username = "User1", Email = "user1@test.com", Password = "pass" };
            var playlists = new List<PlaylistGetDto> { new PlaylistGetDto { Id = "pl1", Title = "PL1" } };

            _mockUserRepo.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(userEntity);
            _mockPlaylistService.Setup(s => s.GetByUserAsync("u1")).ReturnsAsync(playlists);
            _mockPlaylistService.Setup(s => s.DeleteAsync("pl1")).Returns(Task.CompletedTask);
            _mockUserRepo.Setup(r => r.DeleteAsync("u1")).Returns(Task.CompletedTask);

            await _service.DeleteAsync("u1");

            _mockPlaylistService.Verify(s => s.DeleteAsync("pl1"), Times.Once);
            _mockUserRepo.Verify(r => r.DeleteAsync("u1"), Times.Once);
        }

        [Fact]
        // Перевіряє, що GetAsync викидає NullReferenceException для неіснуючого Id
        public async Task GetAsync_ShouldThrowException_WhenUserNotFound()
        {
            _mockUserRepo.Setup(r => r.GetByIdAsync("invalid")).ReturnsAsync((User)null);

            await Assert.ThrowsAsync<NullReferenceException>(() => _service.GetAsync("invalid"));
        }

        [Fact]
        // Перевіряє, що UpdateAsync викидає NullReferenceException для неіснуючого користувача
        public async Task UpdateAsync_ShouldThrowException_WhenUserNotFound()
        {
            var userCreate = new UserCreateDto { Username = "User", Email = "user@test.com", Password = "pass" };
            _mockUserRepo.Setup(r => r.GetByIdAsync("invalid")).ReturnsAsync((User)null);

            await Assert.ThrowsAsync<NullReferenceException>(() => _service.UpdateAsync("invalid", userCreate));
        }

        [Fact]
        // Перевіряє, що DeleteAsync викидає NullReferenceException для неіснуючого користувача
        public async Task DeleteAsync_ShouldThrowException_WhenUserNotFound()
        {
            _mockUserRepo.Setup(r => r.GetByIdAsync("invalid")).ReturnsAsync((User)null);

            await Assert.ThrowsAsync<NullReferenceException>(() => _service.DeleteAsync("invalid"));
        }
    }
}
