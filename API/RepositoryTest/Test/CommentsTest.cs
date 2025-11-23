using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Moq;
using Xunit;
using MusicPlaylistAPI.Models.Entity;
using MusicPlaylistAPI.Repositories;

namespace MusicPlaylistAPI.Tests.Repositories
{
    public class CommentRepositoryTests
    {
        private readonly Mock<IMongoCollection<Comment>> _mockCollection;
        private readonly CommentRepository _repository;

        public CommentRepositoryTests()
        {
            _mockCollection = new Mock<IMongoCollection<Comment>>();
            _repository = new CommentRepository(_mockCollection.Object);
        }

        private Mock<IAsyncCursor<Comment>> CreateCursor(List<Comment> comments)
        {
            var cursor = new Mock<IAsyncCursor<Comment>>();
            cursor.Setup(_ => _.Current).Returns(comments);

            cursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            return cursor;
        }

        [Fact]
        public async Task CreateAsync_ShouldCallInsertOneAsync()
        {
            var comment = new Comment { Id = "1", Title = "Test", Text = "Text" };

            _mockCollection
                .Setup(c => c.InsertOneAsync(comment, null, default))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _repository.CreateAsync(comment);

            _mockCollection.Verify(c => c.InsertOneAsync(comment, null, default), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllComments()
        {
            var comments = new List<Comment>
            {
                new Comment { Id = "1", Title = "A" },
                new Comment { Id = "2", Title = "B" }
            };

            var cursor = CreateCursor(comments);

            _mockCollection
                .Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<Comment>>(),
                    It.IsAny<FindOptions<Comment, Comment>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var result = await _repository.GetAllAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnComment()
        {
            var comment = new Comment { Id = "1", Title = "Test" };
            var cursor = CreateCursor(new List<Comment> { comment });

            _mockCollection
                .Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<Comment>>(),
                    It.IsAny<FindOptions<Comment, Comment>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var result = await _repository.GetByIdAsync("1");

            Assert.NotNull(result);
            Assert.Equal("1", result.Id);
        }

        [Fact]
        public async Task GetByPlaylistIdAsync_ShouldReturnMatchingComments()
        {
            var playlistId = "playlist123";
            var comments = new List<Comment>
            {
                new Comment { Id = "1", PlaylistId = playlistId },
                new Comment { Id = "2", PlaylistId = playlistId }
            };

            var cursor = CreateCursor(comments);

            _mockCollection
                .Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<Comment>>(),
                    It.IsAny<FindOptions<Comment, Comment>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var result = await _repository.GetByPlaylistIdAsync(playlistId);

            Assert.Equal(2, result.Count);
            Assert.All(result, c => Assert.Equal(playlistId, c.PlaylistId));
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallReplaceOneAsync()
        {
            var comment = new Comment { Id = "1", Title = "Updated" };

            _mockCollection
                .Setup(c => c.ReplaceOneAsync(
                    It.IsAny<FilterDefinition<Comment>>(),
                    comment,
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<ReplaceOneResult>())
                .Verifiable();

            await _repository.UpdateAsync("1", comment);

            _mockCollection.Verify(c => c.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Comment>>(),
                comment,
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallDeleteOneAsync()
        {
            _mockCollection
                .Setup(c => c.DeleteOneAsync(
                    It.IsAny<FilterDefinition<Comment>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<DeleteResult>())
                .Verifiable();

            await _repository.DeleteAsync("1");

            _mockCollection.Verify(c => c.DeleteOneAsync(
                It.IsAny<FilterDefinition<Comment>>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
