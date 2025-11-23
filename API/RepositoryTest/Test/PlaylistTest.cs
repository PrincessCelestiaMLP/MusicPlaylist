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
    public class PlaylistRepositoryTests
    {
        private readonly Mock<IMongoCollection<Playlist>> _mockCollection;
        private readonly PlaylistRepository _repository;

        public PlaylistRepositoryTests()
        {
            _mockCollection = new Mock<IMongoCollection<Playlist>>();
            _repository = new PlaylistRepositoryForTest(_mockCollection.Object);
        }

        // Helper to simulate MongoDB IAsyncCursor
        private Mock<IAsyncCursor<Playlist>> CreateCursor(List<Playlist> data)
        {
            var cursor = new Mock<IAsyncCursor<Playlist>>();
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
            var playlist = new Playlist { Id = "1" };

            _mockCollection
                .Setup(c => c.InsertOneAsync(playlist, null, default))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _repository.CreateAsync(playlist);

            _mockCollection.Verify(
                c => c.InsertOneAsync(playlist, null, default),
                Times.Once);
        }

        // 2
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllPlaylists()
        {
            var list = new List<Playlist>
            {
                new Playlist { Id = "1" },
                new Playlist { Id = "2" }
            };

            var cursor = CreateCursor(list);

            _mockCollection.Setup(c =>
                c.FindAsync(It.IsAny<FilterDefinition<Playlist>>(),
                            It.IsAny<FindOptions<Playlist, Playlist>>(),
                            It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var result = await _repository.GetAllAsync();

            Assert.Equal(2, result.Count);
        }

        // 3
        [Fact]
        public async Task GetAllAsync_WhenEmpty_ShouldReturnEmptyList()
        {
            var cursor = CreateCursor(new List<Playlist>());

            _mockCollection.Setup(c =>
                c.FindAsync(It.IsAny<FilterDefinition<Playlist>>(),
                            It.IsAny<FindOptions<Playlist, Playlist>>(),
                            It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var result = await _repository.GetAllAsync();

            Assert.Empty(result);
        }

        // 4
        [Fact]
        public async Task GetByIdAsync_ShouldReturnPlaylist()
        {
            var playlist = new Playlist { Id = "1" };
            var cursor = CreateCursor(new List<Playlist> { playlist });

            _mockCollection
                .Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Playlist>>(),
                                        It.IsAny<FindOptions<Playlist, Playlist>>(),
                                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var result = await _repository.GetByIdAsync("1");

            Assert.NotNull(result);
            Assert.Equal("1", result.Id);
        }

        // 5
        [Fact]
        public async Task GetByIdAsync_WhenNotFound_ShouldReturnNull()
        {
            var cursor = CreateCursor(new List<Playlist>());

            _mockCollection
                .Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Playlist>>(),
                                        It.IsAny<FindOptions<Playlist, Playlist>>(),
                                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var result = await _repository.GetByIdAsync("123");

            Assert.Null(result);
        }

        // 6
        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnUserPlaylists()
        {
            var uid = "user1";
            var list = new List<Playlist>
            {
                new Playlist { Id = "1", UserId = uid },
                new Playlist { Id = "2", UserId = uid }
            };

            var cursor = CreateCursor(list);

            _mockCollection.Setup(c =>
                c.FindAsync(It.IsAny<FilterDefinition<Playlist>>(),
                            It.IsAny<FindOptions<Playlist, Playlist>>(),
                            It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var result = await _repository.GetByUserIdAsync(uid);

            Assert.Equal(2, result.Count);
            Assert.All(result, p => Assert.Equal(uid, p.UserId));
        }

        // 7
        [Fact]
        public async Task GetByUserIdAsync_NoResults_ShouldReturnEmptyList()
        {
            var cursor = CreateCursor(new List<Playlist>());

            _mockCollection.Setup(c =>
                c.FindAsync(It.IsAny<FilterDefinition<Playlist>>(),
                            It.IsAny<FindOptions<Playlist, Playlist>>(),
                            It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var result = await _repository.GetByUserIdAsync("ghost");

            Assert.Empty(result);
        }

        // 8
        [Fact]
        public async Task UpdateAsync_ShouldCallReplaceOneAsync()
        {
            var playlist = new Playlist { Id = "1" };

            _mockCollection
                .Setup(c => c.ReplaceOneAsync(
                    It.IsAny<FilterDefinition<Playlist>>(),
                    playlist,
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<ReplaceOneResult>())
                .Verifiable();

            await _repository.UpdateAsync("1", playlist);

            _mockCollection.Verify(
                c => c.ReplaceOneAsync(
                        It.IsAny<FilterDefinition<Playlist>>(),
                        playlist,
                        It.IsAny<ReplaceOptions>(),
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        // 9
        [Fact]
        public async Task DeleteAsync_ShouldCallDeleteOneAsync()
        {
            _mockCollection
                .Setup(c => c.DeleteOneAsync(
                    It.IsAny<FilterDefinition<Playlist>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<DeleteResult>())
                .Verifiable();

            await _repository.DeleteAsync("1");

            _mockCollection.Verify(
                c => c.DeleteOneAsync(
                        It.IsAny<FilterDefinition<Playlist>>(),
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }

    // Helper class for injecting mocked IMongoCollection
    public class PlaylistRepositoryForTest : PlaylistRepository
    {
        public PlaylistRepositoryForTest(IMongoCollection<Playlist> mockCollection)
        {
            typeof(PlaylistRepository)
                .GetField("_collection",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance)
                .SetValue(this, mockCollection);
        }
    }
}
