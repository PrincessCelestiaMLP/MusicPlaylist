using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Models.Dto.Get;
using MusicPlaylistAPI.Models.Entity;
using MusicPlaylistAPI.Repositories.Interface;
using MusicPlaylistAPI.Services;
using Xunit;

namespace MusicPlaylistAPI.Tests.Services
{
    public class MusicServiceTests
    {
        private readonly Mock<IMusicRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly MusicService _service;

        public MusicServiceTests()
        {
            _mockRepo = new Mock<IMusicRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new MusicService(_mockRepo.Object, _mockMapper.Object);
        }

        [Fact]
        // Перевіряє, що метод CreateAsync правильно створює музичний запис і повертає MusicGetDto
        public async Task CreateAsync_ShouldReturnMusicGetDto()
        {
            var musicCreate = new MusicCreateDto { Title = "Song1", Artist = "Artist1", Link = "link1", PlaylistId = "pl1" };
            var musicEntity = new Music { Id = "m1", Title = "Song1", Artist = "Artist1", Link = "link1", PlaylistId = "pl1" };
            var musicGet = new MusicGetDto { Id = "m1", Title = "Song1", Artist = "Artist1", Link = "link1" };

            _mockMapper.Setup(m => m.Map<Music>(musicCreate)).Returns(musicEntity);
            _mockRepo.Setup(r => r.CreateAsync(musicEntity)).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.GetByIdAsync("m1")).ReturnsAsync(musicEntity);
            _mockMapper.Setup(m => m.Map<MusicGetDto>(musicEntity)).Returns(musicGet);

            var result = await _service.CreateAsync(musicCreate);

            Assert.NotNull(result);
            Assert.Equal("m1", result.Id);
            Assert.Equal("Song1", result.Title);
        }

        [Fact]
        // Перевіряє, що GetAsync повертає всі музичні записи
        public async Task GetAsync_ShouldReturnListOfMusicGetDto()
        {
            var musics = new List<Music> { new Music { Id = "m1", Title = "Song1", Artist = "Artist1", Link = "link1", PlaylistId = "pl1" } };
            var musicsGet = new List<MusicGetDto> { new MusicGetDto { Id = "m1", Title = "Song1", Artist = "Artist1", Link = "link1" } };

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(musics);
            _mockMapper.Setup(m => m.Map<List<MusicGetDto>>(musics)).Returns(musicsGet);

            var result = await _service.GetAsync();

            Assert.Single(result);
            Assert.Equal("m1", result[0].Id);
        }

        [Fact]
        // Перевіряє, що GetAsync(string id) повертає правильний запис за Id
        public async Task GetAsync_ById_ShouldReturnMusicGetDto()
        {
            var musicEntity = new Music { Id = "m1", Title = "Song1", Artist = "Artist1", Link = "link1", PlaylistId = "pl1" };
            var musicGet = new MusicGetDto { Id = "m1", Title = "Song1", Artist = "Artist1", Link = "link1" };

            _mockRepo.Setup(r => r.GetByIdAsync("m1")).ReturnsAsync(musicEntity);
            _mockMapper.Setup(m => m.Map<MusicGetDto>(musicEntity)).Returns(musicGet);

            var result = await _service.GetAsync("m1");

            Assert.NotNull(result);
            Assert.Equal("m1", result.Id);
        }

        [Fact]
        // Перевіряє, що UpdateAsync правильно оновлює запис і повертає оновлений MusicGetDto
        public async Task UpdateAsync_ShouldReturnUpdatedMusicGetDto()
        {
            var musicCreate = new MusicCreateDto { Title = "SongUpdated", Artist = "ArtistUpdated", Link = "linkUpdated", PlaylistId = "pl1" };
            var musicEntity = new Music { Id = "m1", Title = "SongUpdated", Artist = "ArtistUpdated", Link = "linkUpdated", PlaylistId = "pl1" };
            var musicGet = new MusicGetDto { Id = "m1", Title = "SongUpdated", Artist = "ArtistUpdated", Link = "linkUpdated" };

            _mockRepo.Setup(r => r.GetByIdAsync("m1")).ReturnsAsync(musicEntity);
            _mockMapper.Setup(m => m.Map<Music>(musicCreate)).Returns(musicEntity);
            _mockRepo.Setup(r => r.UpdateAsync("m1", musicEntity)).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<MusicGetDto>(musicEntity)).Returns(musicGet);

            var result = await _service.UpdateAsync("m1", musicCreate);

            Assert.Equal("SongUpdated", result.Title);
            Assert.Equal("ArtistUpdated", result.Artist);
        }

        [Fact]
        // Перевіряє, що DeleteAsync викликає метод видалення репозиторію
        public async Task DeleteAsync_ShouldCallDelete()
        {
            var musicEntity = new Music { Id = "m1", Title = "Song1", Artist = "Artist1", Link = "link1", PlaylistId = "pl1" };
            _mockRepo.Setup(r => r.GetByIdAsync("m1")).ReturnsAsync(musicEntity);
            _mockRepo.Setup(r => r.DeleteAsync("m1")).Returns(Task.CompletedTask);

            await _service.DeleteAsync("m1");

            _mockRepo.Verify(r => r.DeleteAsync("m1"), Times.Once);
        }

        [Fact]
        // Перевіряє, що GetAsync(string id) викидає NullReferenceException для неіснуючого Id
        public async Task GetAsync_ById_ShouldThrowException_WhenMusicNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync("invalid")).ReturnsAsync((Music)null);

            await Assert.ThrowsAsync<NullReferenceException>(() => _service.GetAsync("invalid"));
        }

        [Fact]
        // Перевіряє, що UpdateAsync викидає NullReferenceException для неіснуючого запису
        public async Task UpdateAsync_ShouldThrowException_WhenMusicNotFound()
        {
            var musicCreate = new MusicCreateDto { Title = "SongUpdated", Artist = "ArtistUpdated", Link = "linkUpdated", PlaylistId = "pl1" };
            _mockRepo.Setup(r => r.GetByIdAsync("invalid")).ReturnsAsync((Music)null);

            await Assert.ThrowsAsync<NullReferenceException>(() => _service.UpdateAsync("invalid", musicCreate));
        }

        [Fact]
        // Перевіряє, що DeleteAsync викидає NullReferenceException для неіснуючого запису
        public async Task DeleteAsync_ShouldThrowException_WhenMusicNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync("invalid")).ReturnsAsync((Music)null);

            await Assert.ThrowsAsync<NullReferenceException>(() => _service.DeleteAsync("invalid"));
        }

        [Fact]
        // Перевіряє, що GetByPlaylistAsync повертає список музики для певного плейлисту
        public async Task GetByPlaylistAsync_ShouldReturnListOfMusicForPlaylist()
        {
            var playlistId = "pl1";
            var musics = new List<Music>
            {
                new Music { Id = "m1", Title = "Song1", Artist = "Artist1", Link = "link1", PlaylistId = playlistId }
            };
            var musicsGet = new List<MusicGetDto>
            {
                new MusicGetDto { Id = "m1", Title = "Song1", Artist = "Artist1", Link = "link1" }
            };

            _mockRepo.Setup(r => r.GetByPlaylistIdAsync(playlistId)).ReturnsAsync(musics);
            _mockMapper.Setup(m => m.Map<List<MusicGetDto>>(musics)).Returns(musicsGet);

            var result = await _service.GetByPlaylistAsync(playlistId);

            Assert.Single(result);
            Assert.Equal("m1", result[0].Id);
        }
    }
}
