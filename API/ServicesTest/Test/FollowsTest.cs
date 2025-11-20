using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using MusicPlaylistAPI.Models;
using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Models.Dto.Get;
using MusicPlaylistAPI.Models.Entity;
using MusicPlaylistAPI.Repositories.Interface;
using MusicPlaylistAPI.Services;
using Xunit;

namespace MusicPlaylistAPI.Tests.Services
{
    public class FollowServiceTests
    {
        private readonly Mock<IFollowRepository> _mockFollowRepo;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly FollowService _service;

        public FollowServiceTests()
        {
            _mockFollowRepo = new Mock<IFollowRepository>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new FollowService(_mockFollowRepo.Object, _mockUserRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnFollowGetDto()
        {
            // Arrange
            var followCreateDto = new FollowCreteDto { FollowerId = "u1", PlaylistId = "pl1" };
            var followEntity = new Follow { UserId = "u1", PlaylistId = "pl1" };
            var user = new User { Id = "u1", Username = "User1", Email = "user1@test.com" };
            var followGet = new FollowGetDto
            {
                Id = "generated-id",
                Follower = new UserView { Id = "u1", Username = "User1", Email = "user1@test.com" },
                FollowedAt = DateTime.UtcNow
            };

            _mockMapper.Setup(m => m.Map<Follow>(followCreateDto)).Returns(() =>
            {
                followEntity.Id = "generated-id";
                followEntity.FollowedAt = followGet.FollowedAt;
                return followEntity;
            });
            _mockFollowRepo.Setup(r => r.CreateAsync(followEntity)).Returns(Task.CompletedTask);
            _mockFollowRepo.Setup(r => r.GetByIdAsync("generated-id")).ReturnsAsync(followEntity);
            _mockUserRepo.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<FollowGetDto>(followEntity)).Returns(followGet);
            _mockMapper.Setup(m => m.Map<UserView>(user)).Returns(followGet.Follower);

            // Act
            var result = await _service.CreateAsync(followCreateDto);

            // Assert
            Assert.Equal("generated-id", result.Id);
            Assert.Equal("User1", result.Follower.Username);
            Assert.Equal(followGet.FollowedAt, result.FollowedAt);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnListOfFollowGetDto()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var follows = new List<Follow>
            {
                new Follow { Id = "f1", UserId = "u1", PlaylistId = "pl1", FollowedAt = now }
            };
            var user = new User { Id = "u1", Username = "User1", Email = "user1@test.com" };
            var followGet = new FollowGetDto
            {
                Id = "f1",
                Follower = new UserView { Id = "u1", Username = "User1", Email = "user1@test.com" },
                FollowedAt = now
            };

            _mockFollowRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(follows);
            _mockUserRepo.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<List<FollowGetDto>>(follows)).Returns(new List<FollowGetDto> { followGet });
            _mockMapper.Setup(m => m.Map<UserView>(user)).Returns(followGet.Follower);

            // Act
            var result = await _service.GetAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("f1", result[0].Id);
            Assert.Equal("User1", result[0].Follower.Username);
        }

        [Fact]
        public async Task GetAsync_ById_ShouldReturnFollowGetDto()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var follow = new Follow { Id = "f1", UserId = "u1", PlaylistId = "pl1", FollowedAt = now };
            var user = new User { Id = "u1", Username = "User1", Email = "user1@test.com" };
            var followGet = new FollowGetDto
            {
                Id = "f1",
                Follower = new UserView { Id = "u1", Username = "User1", Email = "user1@test.com" },
                FollowedAt = now
            };

            _mockFollowRepo.Setup(r => r.GetByIdAsync("f1")).ReturnsAsync(follow);
            _mockUserRepo.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<FollowGetDto>(follow)).Returns(followGet);
            _mockMapper.Setup(m => m.Map<UserView>(user)).Returns(followGet.Follower);

            // Act
            var result = await _service.GetAsync("f1");

            // Assert
            Assert.Equal("f1", result.Id);
            Assert.Equal("User1", result.Follower.Username);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallRepositoryDelete()
        {
            // Arrange
            var follow = new Follow { Id = "f1", UserId = "u1", PlaylistId = "pl1" };
            _mockFollowRepo.Setup(r => r.GetByIdAsync("f1")).ReturnsAsync(follow);
            _mockFollowRepo.Setup(r => r.DeleteAsync("f1")).Returns(Task.CompletedTask);

            // Act
            await _service.DeleteAsync("f1");

            // Assert
            _mockFollowRepo.Verify(r => r.DeleteAsync("f1"), Times.Once);
        }

        // ----------------------- Додаткові тести -----------------------

        [Fact]
        public async Task GetAsync_ShouldThrowArgumentException_WhenUserNotFound()
        {
            // Arrange
            var follows = new List<Follow>
            {
                new Follow { Id = "f1", UserId = "u1", PlaylistId = "pl1", FollowedAt = DateTime.UtcNow }
            };
            _mockFollowRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(follows);
            _mockUserRepo.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync((User)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.GetAsync());
            Assert.Contains("User with id:u1 doesn't exists", ex.Message);
        }

        [Fact]
        public async Task GetAsync_ById_ShouldThrowNullReferenceException_WhenFollowNotFound()
        {
            // Arrange
            _mockFollowRepo.Setup(r => r.GetByIdAsync("f1")).ReturnsAsync((Follow)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<NullReferenceException>(() => _service.GetAsync("f1"));
            Assert.Contains("Follow with id:f1 doesn't exist", ex.Message);
        }

        [Fact]
        public async Task GetAsync_ById_ShouldThrowArgumentException_WhenUserNotFound()
        {
            // Arrange
            var follow = new Follow { Id = "f1", UserId = "u1", PlaylistId = "pl1", FollowedAt = DateTime.UtcNow };
            _mockFollowRepo.Setup(r => r.GetByIdAsync("f1")).ReturnsAsync(follow);
            _mockUserRepo.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync((User)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.GetAsync("f1"));
            Assert.Contains("User with id:u1 doesn't exists", ex.Message);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowNullReferenceException_WhenFollowNotFound()
        {
            // Arrange
            _mockFollowRepo.Setup(r => r.GetByIdAsync("f1")).ReturnsAsync((Follow)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<NullReferenceException>(() => _service.DeleteAsync("f1"));
            Assert.Contains("Follow with id:f1 doesn't exist", ex.Message);
        }

        [Fact]
        public async Task GetByPlaylistAsync_ShouldReturnFollows()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var follows = new List<Follow>
            {
                new Follow { Id = "f1", UserId = "u1", PlaylistId = "pl1", FollowedAt = now }
            };
            var user = new User { Id = "u1", Username = "User1", Email = "user1@test.com" };
            var followGet = new FollowGetDto
            {
                Id = "f1",
                Follower = new UserView { Id = "u1", Username = "User1", Email = "user1@test.com" },
                FollowedAt = now
            };

            _mockFollowRepo.Setup(r => r.GetByPlaylistIdAsync("pl1")).ReturnsAsync(follows);
            _mockUserRepo.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<List<FollowGetDto>>(follows)).Returns(new List<FollowGetDto> { followGet });
            _mockMapper.Setup(m => m.Map<UserView>(user)).Returns(followGet.Follower);

            // Act
            var result = await _service.GetByPlaylistAsync("pl1");

            // Assert
            Assert.Single(result);
            Assert.Equal("f1", result[0].Id);
            Assert.Equal("User1", result[0].Follower.Username);
        }
    }
}
