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
    public class FollowRepositoryTests
    {
        private readonly Mock<IMongoCollection<Follow>> _mockCollection;
        private readonly FollowRepository _repository;

        public FollowRepositoryTests()
        {
            _mockCollection = new Mock<IMongoCollection<Follow>>();
            _repository = new FollowRepositoryForTest(_mockCollection.Object);
        }

        private Mock<IAsyncCursor<Follow>> CreateCursor(List<Follow> data)
        {
            var cursor = new Mock<IAsyncCursor<Follow>>();
            cursor.Setup(c => c.Current).Returns(data);
            cursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            return cursor;
        }

        // 1
        [Fact]
        public async Task CreateAsync_ShouldCallInsertOneAsync()
        {
            var follow = new Follow { Id = "1" };

            _mockCollection
                .Setup(c => c.InsertOneAsync(follow, null, default))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _repository.CreateAsync(follow);

            _mockCollection.Verify(c => c.InsertOneAsync(follow, null, default), Times.Once);
        }

        // 2
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllFollows()
        {
            var list = new List<Follow>
            {
                new Follow {Id = "1"},
                new Follow {Id = "2"}
            };

            var cursor = CreateCursor(list);

            _mockCollection.Setup(c =>
                c.FindAsync(It.IsAny<FilterDefinition<Follow>>(),
                    It.IsAny<FindOptions<Follow, Follow>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var result = await _repository.GetAllAsync();

            Assert.Equal(2, result.Count);
        }

        // 3
        [Fact]
        public async Task GetAllAsync_EmptyResult_ShouldReturnEmptyList()
        {
            var cursor = CreateCursor(new List<Follow>());

            _mockCollection.Setup(c =>
                c.FindAsync(It.IsAny<FilterDefinition<Follow>>(),
                    It.IsAny<FindOptions<Follow, Follow>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var result = await _repository.GetAllAsync();

            Assert.Empty(result);
        }

        // 4
        [Fact]
        public async Task GetByIdAsync_ShouldReturnFollow()
        {
            var follow = new Follow { Id = "1" };
            var cursor = CreateCursor(new List<Follow> { follow });

            _mockCollection.Setup(c =>
                c.FindAsync(It.IsAny<FilterDefinition<Follow>>(),
                    It.IsAny<FindOptions<Follow, Follow>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var result = await _repository.GetByIdAsync("1");

            Assert.NotNull(result);
            Assert.Equal("1", result.Id);
        }

        // 5
        [Fact]
        public async Task GetByIdAsync_NotFound_ShouldReturnNull()
        {
            var cursor = CreateCursor(new List<Follow>());

            _mockCollection.Setup(c =>
                c.FindAsync(It.IsAny<FilterDefinition<Follow>>(),
                    It.IsAny<FindOptions<Follow, Follow>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var result = await _repository.GetByIdAsync("123");

            Assert.Null(result);
        }

        // 6
        [Fact]
        public async Task GetByPlaylistIdAsync_ShouldReturnMatching()
        {
            var playlistId = "P123";
            var list = new List<Follow>
            {
                new Follow {Id = "1", PlaylistId = playlistId},
                new Follow {Id = "2", PlaylistId = playlistId},
            };

            var cursor = CreateCursor(list);

            _mockCollection.Setup(c =>
                c.FindAsync(It.IsAny<FilterDefinition<Follow>>(),
                    It.IsAny<FindOptions<Follow, Follow>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var result = await _repository.GetByPlaylistIdAsync(playlistId);

            Assert.Equal(2, result.Count);
            Assert.All(result, f => Assert.Equal(playlistId, f.PlaylistId));
        }

        // 7
        [Fact]
        public async Task GetByPlaylistIdAsync_NoMatches_ShouldReturnEmptyList()
        {
            var cursor = CreateCursor(new List<Follow>());

            _mockCollection.Setup(c =>
                c.FindAsync(It.IsAny<FilterDefinition<Follow>>(),
                    It.IsAny<FindOptions<Follow, Follow>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var result = await _repository.GetByPlaylistIdAsync("P999");

            Assert.Empty(result);
        }

        // 8
        [Fact]
        public async Task DeleteAsync_ShouldCallDeleteOneAsync()
        {
            _mockCollection.Setup(c =>
                c.DeleteOneAsync(It.IsAny<FilterDefinition<Follow>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<DeleteResult>())
                .Verifiable();

            await _repository.DeleteAsync("1");

            _mockCollection.Verify(c =>
                c.DeleteOneAsync(It.IsAny<FilterDefinition<Follow>>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        // 9
        [Fact]
        public async Task DeleteAsync_ShouldNotThrow_WhenNothingDeleted()
        {
            _mockCollection.Setup(c =>
                c.DeleteOneAsync(It.IsAny<FilterDefinition<Follow>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(DeleteResult.Unacknowledged.Instance);

            var exception = await Record.ExceptionAsync(() => _repository.DeleteAsync("DoesNotExist"));

            Assert.Null(exception);
        }
    }

    // Helper class for injecting mocks (same pattern as CommentRepository)
    public class FollowRepositoryForTest : FollowRepository
    {
        public FollowRepositoryForTest(IMongoCollection<Follow> collection)
        {
            typeof(FollowRepository)
                .GetField("_collection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(this, collection);
        }
    }
}
