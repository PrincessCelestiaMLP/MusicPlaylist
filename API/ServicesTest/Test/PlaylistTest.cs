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
    public class PlaylistServiceTests
    {
        private readonly Mock<IPlaylistRepository> _mockRepo;
        private readonly Mock<IMusicService> _mockMusicService;
        private readonly Mock<ICommentService> _mockCommentService;
        private readonly Mock<IFollowService> _mockFollowService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly PlaylistService _service;

        public PlaylistServiceTests()
        {
            _mockRepo = new Mock<IPlaylistRepository>();
            _mockMusicService = new Mock<IMusicService>();
            _mockCommentService = new Mock<ICommentService>();
            _mockFollowService = new Mock<IFollowService>();
            _mockMapper = new Mock<IMapper>();
            _service = new PlaylistService(_mockRepo.Object, _mockMusicService.Object, _mockCommentService.Object,
                                           _mockFollowService.Object, _mockMapper.Object);
        }

        [Fact]
        // Перевіряє створення плейлисту і повернення PlaylistGetDto
        public async Task CreateAsync_ShouldReturnPlaylistGetDto()
        {
            var playlistCreate = new PlaylistCreateDto { Title = "PL1", Cover = new byte[0], UserId = "u1" };
            var playlistEntity = new Playlist { Id = "pl1", Title = "PL1", Cover = new byte[0], UserId = "u1", CreatedAt = DateTime.UtcNow };
            var playlistGet = new PlaylistGetDto { Id = "pl1", Title = "PL1", Cover = new byte[0] };

            _mockMapper.Setup(m => m.Map<Playlist>(playlistCreate)).Returns(playlistEntity);
            _mockRepo.Setup(r => r.CreateAsync(playlistEntity)).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.GetByIdAsync("pl1")).ReturnsAsync(playlistEntity);
            _mockMapper.Setup(m => m.Map<PlaylistGetDto>(playlistEntity)).Returns(playlistGet);

            var result = await _service.CreateAsync(playlistCreate);

            Assert.NotNull(result);
            Assert.Equal("pl1", result.Id);
            Assert.Equal("PL1", result.Title);
        }

        [Fact]
        // Перевіряє отримання всіх плейлистів
        public async Task GetAsync_ShouldReturnListOfPlaylists()
        {
            var playlists = new List<Playlist> { new Playlist { Id = "pl1", Title = "PL1", UserId = "u1", CreatedAt = DateTime.UtcNow } };
            var playlistsGet = new List<PlaylistGetDto> { new PlaylistGetDto { Id = "pl1", Title = "PL1" } };

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(playlists);
            _mockMapper.Setup(m => m.Map<List<PlaylistGetDto>>(playlists)).Returns(playlistsGet);

            _mockMusicService.Setup(s => s.GetByPlaylistAsync("pl1")).ReturnsAsync(new List<MusicGetDto>());
            _mockCommentService.Setup(s => s.GetByPlaylistAsync("pl1")).ReturnsAsync(new List<CommentGetDto>());
            _mockFollowService.Setup(s => s.GetByPlaylistAsync("pl1")).ReturnsAsync(new List<FollowGetDto>());

            var result = await _service.GetAsync();

            Assert.Single(result);
            Assert.Equal("pl1", result[0].Id);
            Assert.NotNull(result[0].Musics);
            Assert.NotNull(result[0].Comments);
            Assert.NotNull(result[0].Follows);
        }

        [Fact]
        // Перевіряє отримання плейлисту за Id
        public async Task GetAsync_ById_ShouldReturnPlaylist()
        {
            var playlistEntity = new Playlist { Id = "pl1", Title = "PL1", UserId = "u1", CreatedAt = DateTime.UtcNow };
            var playlistGet = new PlaylistGetDto { Id = "pl1", Title = "PL1" };

            _mockRepo.Setup(r => r.GetByIdAsync("pl1")).ReturnsAsync(playlistEntity);
            _mockMapper.Setup(m => m.Map<PlaylistGetDto>(playlistEntity)).Returns(playlistGet);

            _mockMusicService.Setup(s => s.GetByPlaylistAsync("pl1")).ReturnsAsync(new List<MusicGetDto>());
            _mockCommentService.Setup(s => s.GetByPlaylistAsync("pl1")).ReturnsAsync(new List<CommentGetDto>());
            _mockFollowService.Setup(s => s.GetByPlaylistAsync("pl1")).ReturnsAsync(new List<FollowGetDto>());

            var result = await _service.GetAsync("pl1");

            Assert.Equal("pl1", result.Id);
        }

        [Fact]
        // Перевіряє отримання плейлистів користувача
        public async Task GetByUserAsync_ShouldReturnUserPlaylists()
        {
            var playlists = new List<Playlist> { new Playlist { Id = "pl1", Title = "PL1", UserId = "u1", CreatedAt = DateTime.UtcNow } };
            var playlistsGet = new List<PlaylistGetDto> { new PlaylistGetDto { Id = "pl1", Title = "PL1" } };

            _mockRepo.Setup(r => r.GetByUserIdAsync("u1")).ReturnsAsync(playlists);
            _mockMapper.Setup(m => m.Map<List<PlaylistGetDto>>(playlists)).Returns(playlistsGet);

            _mockMusicService.Setup(s => s.GetByPlaylistAsync("pl1")).ReturnsAsync(new List<MusicGetDto>());
            _mockCommentService.Setup(s => s.GetByPlaylistAsync("pl1")).ReturnsAsync(new List<CommentGetDto>());
            _mockFollowService.Setup(s => s.GetByPlaylistAsync("pl1")).ReturnsAsync(new List<FollowGetDto>());

            var result = await _service.GetByUserAsync("u1");

            Assert.Single(result);
            Assert.Equal("pl1", result[0].Id);
        }

        [Fact]
        // Перевіряє оновлення плейлисту
        public async Task UpdateAsync_ShouldReturnUpdatedPlaylist()
        {
            var playlistCreate = new PlaylistCreateDto { Title = "UpdatedPL", Cover = new byte[0], UserId = "u1" };
            var playlistEntity = new Playlist { Id = "pl1", Title = "UpdatedPL", UserId = "u1", CreatedAt = DateTime.UtcNow };

            _mockRepo.Setup(r => r.GetByIdAsync("pl1")).ReturnsAsync(playlistEntity);
            _mockMapper.Setup(m => m.Map<Playlist>(playlistCreate)).Returns(playlistEntity);
            _mockRepo.Setup(r => r.UpdateAsync("pl1", playlistEntity)).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<PlaylistGetDto>(playlistCreate)).Returns(new PlaylistGetDto { Id = "pl1", Title = "UpdatedPL" });

            var result = await _service.UpdateAsync("pl1", playlistCreate);

            Assert.Equal("pl1", result.Id);
            Assert.Equal("UpdatedPL", result.Title);
        }

        [Fact]
        // Перевіряє видалення плейлисту
        public async Task DeleteAsync_ShouldCallDeleteDependencies()
        {
            var playlistEntity = new Playlist { Id = "pl1", Title = "PL1", UserId = "u1" };
            var musics = new List<MusicGetDto> { new MusicGetDto { Id = "m1", Title = "Song" } };
            var comments = new List<CommentGetDto> { new CommentGetDto { Id = "c1", Title = "Comment", Text = "Text" } };
            var follows = new List<FollowGetDto> { new FollowGetDto { Id = "f1", FollowedAt = DateTime.UtcNow } };

            _mockRepo.Setup(r => r.GetByIdAsync("pl1")).ReturnsAsync(playlistEntity);
            _mockMusicService.Setup(s => s.GetByPlaylistAsync("pl1")).ReturnsAsync(musics);
            _mockCommentService.Setup(s => s.GetByPlaylistAsync("pl1")).ReturnsAsync(comments);
            _mockFollowService.Setup(s => s.GetByPlaylistAsync("pl1")).ReturnsAsync(follows);
            _mockMusicService.Setup(s => s.DeleteAsync("m1")).Returns(Task.CompletedTask);
            _mockCommentService.Setup(s => s.DeleteAsync("c1")).Returns(Task.CompletedTask);
            _mockFollowService.Setup(s => s.DeleteAsync("f1")).Returns(Task.CompletedTask);

            await _service.DeleteAsync("pl1");

            _mockMusicService.Verify(s => s.DeleteAsync("m1"), Times.Once);
            _mockCommentService.Verify(s => s.DeleteAsync("c1"), Times.Once);
            _mockFollowService.Verify(s => s.DeleteAsync("f1"), Times.Once);
        }

        [Fact]
        // Перевіряє, що GetAsync викидає NullReferenceException для неіснуючого Id
        public async Task GetAsync_ShouldThrowException_WhenPlaylistNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync("invalid")).ReturnsAsync((Playlist)null);

            await Assert.ThrowsAsync<NullReferenceException>(() => _service.GetAsync("invalid"));
        }

        [Fact]
        // Перевіряє, що UpdateAsync викидає NullReferenceException для неіснуючого плейлисту
        public async Task UpdateAsync_ShouldThrowException_WhenPlaylistNotFound()
        {
            var playlistCreate = new PlaylistCreateDto { Title = "UpdatedPL", Cover = new byte[0], UserId = "u1" };
            _mockRepo.Setup(r => r.GetByIdAsync("invalid")).ReturnsAsync((Playlist)null);

            await Assert.ThrowsAsync<NullReferenceException>(() => _service.UpdateAsync("invalid", playlistCreate));
        }

        [Fact]
        // Перевіряє, що DeleteAsync викидає NullReferenceException для неіснуючого плейлисту
        public async Task DeleteAsync_ShouldThrowException_WhenPlaylistNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync("invalid")).ReturnsAsync((Playlist)null);

            await Assert.ThrowsAsync<NullReferenceException>(() => _service.DeleteAsync("invalid"));
        }
    }
}
