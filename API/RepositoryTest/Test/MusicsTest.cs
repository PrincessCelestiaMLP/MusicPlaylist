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
    public class MusicRepositoryTests
    {
        private readonly Mock<IMongoCollection<Music>> _mockCollection;
        private readonly MusicRepository _repository;

        public MusicRepositoryTests()
        {
            _mockCollection = new Mock<IMongoCollection<Music>>();
            _repository = new MusicRepositoryForTest(_mockCollection.Object);
        }

        private Mock<IAsyncCursor<Music>> CreateCursor(List<Music> data)
        {
            var cursor = new Mock<IAsyncCursor<Music>>();
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
            var music = new Music { Id = "1" };

            _mockCollection
                .Setup(c => c.InsertOneAsync(music, null, default))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _repository.CreateAsync(music);

            _mockCollection.Verify(c => c.InsertOneAsync(music, null, default), Times.Once);
        }

        // 2
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllMusic()
        {
            var list = new List<Music>
            {
                new Music {Id = "1"},
                new Music {Id = "2"}
            };

            var cursor = CreateCursor(list);

            _mockCollection.Setup(c =>
                c.FindAsync(It.IsAny<FilterDefinition<Music>>(),
                    It.IsAny<FindOptions<Music, Music>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var result = await _repository.GetAllAsync();

            Assert.Equal(2, result.Count);
        }

        // 3
        [Fact]
        public async Task GetAllAsync_EmptyResult_ShouldReturnEmptyList()
        {
            var cursor = CreateCursor(new List<Music>());

            _mockCollection.Setup(c =>
                c.FindAsync(It.IsAny<FilterDefinition<Music>>(),
                    It.IsAny<FindOptions<Music, Music>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var result = await _repository.GetAllAsync();

            Assert.Empty(result);
        }

        // 4
        [Fact]
        public async Task GetByIdAsync_ShouldReturnMusic()
        {
            var music = new Music { Id = "1" };
            var cursor = CreateCursor(new List<Music> { music });

            _mockCollection.Setup(c =>
                c.FindAsync(It.IsAny<FilterDefinition<Music>>(),
                    It.IsAny<FindOptions<Music, Music>>(),
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
            var cursor = CreateCursor(new List<Music>());

            _mockCollection.Setup(c =>
                c.FindAsync(It.IsAny<FilterDefinition<Music>>(),
                    It.IsAny<FindOptions<Music, Music>>(),
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
            var list = new List<Music>
            {
                new Music {Id = "1", PlaylistId = playlistId},
                new Music {Id = "2", PlaylistId = playlistId}
            };

            var cursor = CreateCursor(list);

            _mockCollection.Setup(c =>
                c.FindAsync(It.IsAny<FilterDefinition<Music>>(),
                    It.IsAny<FindOptions<Music, Music>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var result = await _repository.GetByPlaylistIdAsync(playlistId);

            Assert.Equal(2, result.Count);
            Assert.All(result, m => Assert.Equal(playlistId, m.PlaylistId));
        }

        // 7
        [Fact]
        public async Task GetByPlaylistIdAsync_NoMatches_ShouldReturnEmptyList()
        {
            var cursor = CreateCursor(new List<Music>());

            _mockCollection.Setup(c =>
                c.FindAsync(It.IsAny<FilterDefinition<Music>>(),
                    It.IsAny<FindOptions<Music, Music>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var result = await _repository.GetByPlaylistIdAsync("P999");

            Assert.Empty(result);
        }

        // 8
        [Fact]
        public async Task UpdateAsync_ShouldCallReplaceOneAsync()
        {
            var music = new Music { Id = "1" };

            _mockCollection.Setup(c =>
                c.ReplaceOneAsync(It.IsAny<FilterDefinition<Music>>(),
                    music,
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<ReplaceOneResult>())
                .Verifiable();

            await _repository.UpdateAsync("1", music);

            _mockCollection.Verify(c =>
                c.ReplaceOneAsync(It.IsAny<FilterDefinition<Music>>(),
                    music,
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        // 9
        [Fact]
        public async Task DeleteAsync_ShouldCallDeleteOneAsync()
        {
            _mockCollection.Setup(c =>
                c.DeleteOneAsync(It.IsAny<FilterDefinition<Music>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<DeleteResult>())
                .Verifiable();

            await _repository.DeleteAsync("1");

            _mockCollection.Verify(c =>
                c.DeleteOneAsync(It.IsAny<FilterDefinition<Music>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }

    // Helper to inject mocks
    public class MusicRepositoryForTest : MusicRepository
    {
        public MusicRepositoryForTest(IMongoCollection<Music> collection)
        {
            typeof(MusicRepository)
                .GetField("_collection",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance)
                .SetValue(this, collection);
        }
    }
}
