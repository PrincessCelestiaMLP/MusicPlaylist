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
    public class UserRepositoryTests
    {
        private readonly Mock<IMongoCollection<User>> _mockCollection;
        private readonly UserRepository _repository;

        public UserRepositoryTests()
        {
            _mockCollection = new Mock<IMongoCollection<User>>();
            _repository = new UserRepositoryForTest(_mockCollection.Object);
        }

        // Helper: create fake Mongo cursor
        private Mock<IAsyncCursor<User>> CreateCursor(List<User> list)
        {
            var cursor = new Mock<IAsyncCursor<User>>();
            cursor.Setup(c => c.Current).Returns(list);
            cursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            return cursor;
        }

        // 1
        [Fact]
        public async Task CreateAsync_ShouldCallInsertOneAsync()
        {
            var user = new User { Id = "1" };

            _mockCollection
                .Setup(c => c.InsertOneAsync(user, null, default))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _repository.CreateAsync(user);

            _mockCollection.Verify(
                c => c.InsertOneAsync(user, null, default),
                Times.Once);
        }

        // 2
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllUsers()
        {
            var users = new List<User>
            {
                new User { Id = "1" },
                new User { Id = "2" }
            };

            var cursor = CreateCursor(users);

            _mockCollection.Setup(c =>
                c.FindAsync(It.IsAny<FilterDefinition<User>>(),
                            It.IsAny<FindOptions<User, User>>(),
                            It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var result = await _repository.GetAllAsync();

            Assert.Equal(2, result.Count);
        }

        // 3
        [Fact]
        public async Task GetAllAsync_Empty_ReturnsEmptyList()
        {
            var cursor = CreateCursor(new List<User>());

            _mockCollection.Setup(c =>
                c.FindAsync(It.IsAny<FilterDefinition<User>>(),
                            It.IsAny<FindOptions<User, User>>(),
                            It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var result = await _repository.GetAllAsync();

            Assert.Empty(result);
        }

        // 4
        [Fact]
        public async Task GetByIdAsync_Found_ReturnsUser()
        {
            var user = new User { Id = "1" };
            var cursor = CreateCursor(new List<User> { user });

            _mockCollection.Setup(c =>
                c.FindAsync(It.IsAny<FilterDefinition<User>>(),
                            It.IsAny<FindOptions<User, User>>(),
                            It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var result = await _repository.GetByIdAsync("1");

            Assert.NotNull(result);
            Assert.Equal("1", result.Id);
        }

        // 5
        [Fact]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            var cursor = CreateCursor(new List<User>());

            _mockCollection.Setup(c =>
                c.FindAsync(It.IsAny<FilterDefinition<User>>(),
                            It.IsAny<FindOptions<User, User>>(),
                            It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var result = await _repository.GetByIdAsync("999");

            Assert.Null(result);
        }

        // 6
        [Fact]
        public async Task GetByEmailAsync_Found_ReturnsUser()
        {
            var user = new User { Email = "test@mail.com" };
            var cursor = CreateCursor(new List<User> { user });

            _mockCollection.Setup(c =>
                c.FindAsync(It.IsAny<FilterDefinition<User>>(),
                            It.IsAny<FindOptions<User, User>>(),
                            It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var result = await _repository.GetByEmailAsync("test@mail.com");

            Assert.NotNull(result);
            Assert.Equal("test@mail.com", result.Email);
        }

        // 7
        [Fact]
        public async Task GetByEmailAsync_NotFound_ReturnsNull()
        {
            var cursor = CreateCursor(new List<User>());

            _mockCollection.Setup(c =>
                c.FindAsync(It.IsAny<FilterDefinition<User>>(),
                            It.IsAny<FindOptions<User, User>>(),
                            It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var result = await _repository.GetByEmailAsync("unknown@mail.com");

            Assert.Null(result);
        }

        // 8
        [Fact]
        public async Task UpdateAsync_ShouldCallReplaceOneAsync()
        {
            var user = new User { Id = "1" };

            _mockCollection
                .Setup(c => c.ReplaceOneAsync(
                    It.IsAny<FilterDefinition<User>>(),
                    user,
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<ReplaceOneResult>())
                .Verifiable();

            await _repository.UpdateAsync("1", user);

            _mockCollection.Verify(
                c => c.ReplaceOneAsync(
                        It.IsAny<FilterDefinition<User>>(),
                        user,
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
                    It.IsAny<FilterDefinition<User>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<DeleteResult>())
                .Verifiable();

            await _repository.DeleteAsync("1");

            _mockCollection.Verify(
                c => c.DeleteOneAsync(
                        It.IsAny<FilterDefinition<User>>(),
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }

    // Inject mock IMongoCollection using reflection
    public class UserRepositoryForTest : UserRepository
    {
        public UserRepositoryForTest(IMongoCollection<User> mockCollection)
        {
            typeof(UserRepository)
                .GetField("_collection",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance)
                .SetValue(this, mockCollection);
        }
    }
}
