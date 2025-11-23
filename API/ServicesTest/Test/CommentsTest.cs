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
    public class CommentServiceTests
    {
        private readonly Mock<ICommentRepository> _mockCommentRepo;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CommentService _service;

        public CommentServiceTests()
        {
            _mockCommentRepo = new Mock<ICommentRepository>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new CommentService(_mockCommentRepo.Object, _mockUserRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnCommentGetDto()
        {
            // Arrange
            var commentCreate = new CommentCreateDto { Title = "Test", Text = "Test", PlaylistId = "pl1", UserId = "u1" };
            var commentEntity = new Comment { Title = "Test", Text = "Test", PlaylistId = "pl1", UserId = "u1" }; // Id і CreatedAt згенерує Mapper
            var user = new User { Id = "u1", Username = "User1", Email = "user1@test.com" };

            var commentGet = new CommentGetDto
            {
                Id = "generated-id",
                Title = "Test",
                Text = "Test",
                CreatedAt = DateTime.Now,
                Author = new UserView { Id = "u1", Username = "User1", Email = "user1@test.com" }
            };

            _mockMapper.Setup(m => m.Map<Comment>(commentCreate)).Returns(() =>
            {
                commentEntity.Id = "generated-id";
                commentEntity.CreatedAt = commentGet.CreatedAt;
                return commentEntity;
            });
            _mockCommentRepo.Setup(r => r.CreateAsync(commentEntity)).Returns(Task.CompletedTask);
            _mockCommentRepo.Setup(r => r.GetByIdAsync("generated-id")).ReturnsAsync(commentEntity);
            _mockUserRepo.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<CommentGetDto>(commentEntity))
    .Returns(new CommentGetDto
    {
        Id = "generated-id",
        Title = "Test",
        Text = "Test",
        CreatedAt = commentEntity.CreatedAt
    });

            _mockMapper.Setup(m => m.Map<UserView>(user)).Returns(commentGet.Author);

            // Act
            var result = await _service.CreateAsync(commentCreate);

            // Assert
            Assert.Equal("generated-id", result.Id);
            Assert.Equal("Test", result.Title);
            Assert.Equal("User1", result.Author.Username);
            Assert.Equal(commentGet.CreatedAt, result.CreatedAt);
        }


        [Fact]
        public async Task GetAsync_ShouldReturnListOfCommentGetDto()
        {
            // Arrange
            var createdAt = DateTime.UtcNow;
            var comments = new List<Comment>
            {
                new Comment { Id = "c1", Title = "T1", Text="Text1", PlaylistId="pl1", UserId="u1", CreatedAt = createdAt }
            };
            var user = new User { Id = "u1", Username = "User1", Email = "user1@test.com" };
            var commentGet = new CommentGetDto { Id = "c1", Title = "T1", Text = "Text1", CreatedAt = createdAt, Author = new UserView { Id = "u1", Username = "User1", Email = "user1@test.com" } };

            _mockCommentRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(comments);
            _mockUserRepo.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<List<CommentGetDto>>(comments)).Returns(new List<CommentGetDto> { commentGet });
            _mockMapper.Setup(m => m.Map<UserView>(user)).Returns(commentGet.Author);

            // Act
            var result = await _service.GetAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("c1", result[0].Id);
            Assert.Equal("T1", result[0].Title);
            Assert.Equal("User1", result[0].Author.Username);
            Assert.Equal(createdAt, result[0].CreatedAt);
        }

        [Fact]
        public async Task GetAsync_ById_ShouldReturnCommentGetDto()
        {
            // Arrange
            var createdAt = DateTime.UtcNow;
            var comment = new Comment { Id = "c1", Title = "T1", Text = "Text1", PlaylistId = "pl1", UserId = "u1", CreatedAt = createdAt };
            var user = new User { Id = "u1", Username = "User1", Email = "user1@test.com" };
            var commentGet = new CommentGetDto { Id = "c1", Title = "T1", Text = "Text1", CreatedAt = createdAt, Author = new UserView { Id = "u1", Username = "User1", Email = "user1@test.com" } };

            _mockCommentRepo.Setup(r => r.GetByIdAsync("c1")).ReturnsAsync(comment);
            _mockUserRepo.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<CommentGetDto>(comment)).Returns(commentGet);
            _mockMapper.Setup(m => m.Map<UserView>(user)).Returns(commentGet.Author);

            // Act
            var result = await _service.GetAsync("c1");

            // Assert
            Assert.Equal("c1", result.Id);
            Assert.Equal("User1", result.Author.Username);
            Assert.Equal(createdAt, result.CreatedAt);
        }

        [Fact]
        public async Task GetAsync_ById_ShouldThrowNullReferenceException_WhenCommentNotFound()
        {
            // Arrange
            _mockCommentRepo.Setup(r => r.GetByIdAsync("invalid")).ReturnsAsync((Comment)null);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _service.GetAsync("invalid"));
        }

        [Fact]
        public async Task GetByPlaylistAsync_ShouldReturnCommentsWithAuthors()
        {
            // Arrange
            var playlistId = "pl1";
            var createdAt1 = DateTime.UtcNow;
            var createdAt2 = DateTime.UtcNow.AddMinutes(1);

            var comments = new List<Comment>
            {
                new Comment { Id = "c1", Title = "Title1", Text = "Text1", PlaylistId = playlistId, UserId = "u1", CreatedAt = createdAt1 },
                new Comment { Id = "c2", Title = "Title2", Text = "Text2", PlaylistId = playlistId, UserId = "u2", CreatedAt = createdAt2 }
            };

            var user1 = new User { Id = "u1", Username = "User1", Email = "user1@test.com" };
            var user2 = new User { Id = "u2", Username = "User2", Email = "user2@test.com" };

            var commentGet1 = new CommentGetDto { Id = "c1", Title = "Title1", Text = "Text1", CreatedAt = createdAt1, Author = new UserView { Id = "u1", Username = "User1", Email = "user1@test.com" } };
            var commentGet2 = new CommentGetDto { Id = "c2", Title = "Title2", Text = "Text2", CreatedAt = createdAt2, Author = new UserView { Id = "u2", Username = "User2", Email = "user2@test.com" } };

            _mockCommentRepo.Setup(r => r.GetByPlaylistIdAsync(playlistId)).ReturnsAsync(comments);
            _mockUserRepo.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user1);
            _mockUserRepo.Setup(r => r.GetByIdAsync("u2")).ReturnsAsync(user2);
            _mockMapper.Setup(m => m.Map<List<CommentGetDto>>(comments)).Returns(new List<CommentGetDto> { commentGet1, commentGet2 });
            _mockMapper.Setup(m => m.Map<UserView>(user1)).Returns(commentGet1.Author);
            _mockMapper.Setup(m => m.Map<UserView>(user2)).Returns(commentGet2.Author);

            // Act
            var result = await _service.GetByPlaylistAsync(playlistId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("User1", result[0].Author.Username);
            Assert.Equal("User2", result[1].Author.Username);
            Assert.Equal(createdAt1, result[0].CreatedAt);
            Assert.Equal(createdAt2, result[1].CreatedAt);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallDelete()
        {
            // Arrange
            var comment = new Comment { Id = "c1", Title = "T1", Text = "Text1", PlaylistId = "pl1", UserId = "u1" };
            _mockCommentRepo.Setup(r => r.GetByIdAsync("c1")).ReturnsAsync(comment);
            _mockCommentRepo.Setup(r => r.DeleteAsync("c1")).Returns(Task.CompletedTask);

            // Act
            await _service.DeleteAsync("c1");

            // Assert
            _mockCommentRepo.Verify(r => r.DeleteAsync("c1"), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowNullReferenceException_WhenCommentNotFound()
        {
            // Arrange
            _mockCommentRepo.Setup(r => r.GetByIdAsync("invalid")).ReturnsAsync((Comment)null);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _service.DeleteAsync("invalid"));
        }
    }
}
