using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Diagnostic.Inconsistencies.DiscInconsistencies;
using PandaPlayer.Services.IntegrationTests.Data;
using PandaPlayer.Services.IntegrationTests.Extensions;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Services.Interfaces.Dal;
using PandaPlayer.Services.Tagging;

namespace PandaPlayer.Services.IntegrationTests
{
	[TestClass]
	public class SongsServiceTests : BasicServiceTests<ISongsService>
	{
		[TestMethod]
		public async Task CreateSong_ForSongWithAllOptionalPropertiesFilled_CreatesSongSuccessfully()
		{
			// Arrange

			var newSong = new SongModel
			{
				Title = "Дети Галактики",
				TreeTitle = "03 - Дети Галактики.mp3",
				TrackNumber = 3,
				Artist = await GetArtist(ReferenceData.Artist2Id),
				Genre = await GetGenre(ReferenceData.Genre1Id),
				Duration = TimeSpan.FromMilliseconds(12345),
				Rating = RatingModel.R7,
				BitRate = 54321,
			};

			var disc = await GetDisc(ReferenceData.NormalDiscId);
			disc.AddSong(newSong);

			var target = CreateTestTarget();

			// Act

			await using var songContent = File.OpenRead("ContentForAdding/New Song Without Tags.mp3");
			await target.CreateSong(newSong, songContent, CancellationToken.None);

			// Assert

			newSong.Id.Should().Be(ReferenceData.NextSongId);

			var referenceData = GetReferenceData();
			var expectedDisc = referenceData.NormalDisc;
			var expectedSong = new SongModel
			{
				Id = ReferenceData.NextSongId,
				Title = "Дети Галактики",
				TreeTitle = "03 - Дети Галактики.mp3",
				TrackNumber = 3,
				Artist = referenceData.Artist2,
				Genre = referenceData.Genre1,
				Duration = TimeSpan.FromMilliseconds(12345),
				Rating = RatingModel.R7,
				BitRate = 54321,
				Size = 416039,
				Checksum = 2259945390,
				ContentUri = "Belarusian/Neuro Dubel/2010 - Афтары правды (CD 1)/03 - Дети Галактики.mp3".ToContentUri(LibraryStorageRoot),
			};

			expectedDisc.AddSong(expectedSong);

			var discFromRepository = await GetDisc(ReferenceData.NormalDiscId);
			discFromRepository.Should().BeEquivalentTo(expectedDisc, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var songFilePath = Path.Combine(LibraryStorageRoot, "Belarusian", "Neuro Dubel", "2010 - Афтары правды (CD 1)", "03 - Дети Галактики.mp3");
			var fileInfo = new FileInfo(songFilePath);
			fileInfo.Exists.Should().BeTrue();
			fileInfo.Length.Should().Be(416039);

			var expectedTagData = new SongTagData
			{
				Artist = "Neuro Dubel",
				Album = "Афтары правды",
				Year = 2010,
				Genre = "Punk Rock",
				Track = 3,
				Title = "Дети Галактики",
			};

			var songTagger = GetService<ISongTagger>();
			var tagData = songTagger.GetTagData(songFilePath);
			tagData.Should().BeEquivalentTo(expectedTagData, x => x.WithStrictOrdering());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task CreateSong_ForSongWithAllOptionalPropertiesNotFilled_CreatesSongSuccessfully()
		{
			// Arrange

			var newSong = new SongModel
			{
				Title = "Дети Галактики",
				TreeTitle = "03 - Дети Галактики.mp3",
				Duration = TimeSpan.FromMilliseconds(12345),
				BitRate = 54321,
			};

			var disc = await GetDisc(ReferenceData.DiscWithMissingFieldsId);
			disc.AddSong(newSong);

			var target = CreateTestTarget();

			// Act

			await using var songContent = File.OpenRead("ContentForAdding/New Song Without Tags.mp3");
			await target.CreateSong(newSong, songContent, CancellationToken.None);

			// Assert

			newSong.Id.Should().Be(ReferenceData.NextSongId);

			var referenceData = GetReferenceData();
			var expectedDisc = referenceData.DiscWithMissingFields;
			var expectedSong = new SongModel
			{
				Id = ReferenceData.NextSongId,
				Title = "Дети Галактики",
				TreeTitle = "03 - Дети Галактики.mp3",
				Duration = TimeSpan.FromMilliseconds(12345),
				BitRate = 54321,
				Size = 415898,
				Checksum = 3771089602,
				ContentUri = "Belarusian/Neuro Dubel/Disc With Missing Fields (CD 1)/03 - Дети Галактики.mp3".ToContentUri(LibraryStorageRoot),
			};

			expectedDisc.AddSong(expectedSong);

			var discFromRepository = await GetDisc(ReferenceData.DiscWithMissingFieldsId);
			discFromRepository.Should().BeEquivalentTo(expectedDisc, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var songFilePath = Path.Combine(LibraryStorageRoot, "Belarusian", "Neuro Dubel", "Disc With Missing Fields (CD 1)", "03 - Дети Галактики.mp3");
			var fileInfo = new FileInfo(songFilePath);
			fileInfo.Exists.Should().BeTrue();
			fileInfo.Length.Should().Be(415898);

			var expectedTagData = new SongTagData
			{
				Title = "Дети Галактики",
			};

			var songTagger = GetService<ISongTagger>();
			var tagData = songTagger.GetTagData(songFilePath);
			tagData.Should().BeEquivalentTo(expectedTagData, x => x.WithStrictOrdering());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task CreateSong_ForSongContentWithFilledTags_ClearsExistingTags()
		{
			// Arrange

			var newSong = new SongModel
			{
				Title = "Дети Галактики",
				TreeTitle = "03 - Дети Галактики.mp3",
				Duration = TimeSpan.FromMilliseconds(12345),
				BitRate = 54321,
			};

			var disc = await GetDisc(ReferenceData.DiscWithMissingFieldsId);
			disc.AddSong(newSong);

			var target = CreateTestTarget();

			// Act

			await using var songContent = File.OpenRead("ContentForAdding/New Song With Tags.mp3");
			await target.CreateSong(newSong, songContent, CancellationToken.None);

			// Assert

			var songFilePath = Path.Combine(LibraryStorageRoot, "Belarusian", "Neuro Dubel", "Disc With Missing Fields (CD 1)", "03 - Дети Галактики.mp3");

			var expectedTagData = new SongTagData
			{
				Title = "Дети Галактики",
			};

			var songTagger = GetService<ISongTagger>();
			var tagData = songTagger.GetTagData(songFilePath);
			tagData.Should().BeEquivalentTo(expectedTagData, x => x.WithStrictOrdering());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task GetSongs_ForExistingActiveSongIds_ReturnsSongsCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var songIds = new[]
			{
				ReferenceData.SongWithOptionalPropertiesFilledId1,
				ReferenceData.SongWithOptionalPropertiesMissingId,
			};

			var songs = await target.GetSongs(songIds, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedSongs = new[]
			{
				referenceData.SongWithOptionalPropertiesFilled1,
				referenceData.SongWithOptionalPropertiesMissing,
			};

			songs.Should().BeEquivalentTo(expectedSongs, x => x.WithStrictOrdering().IgnoringCyclicReferences());
		}

		[TestMethod]
		public async Task GetSongs_ListContainsDeletedSongs_ReturnsDeletedSongs()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var songIds = new[]
			{
				ReferenceData.SongWithOptionalPropertiesFilledId1,
				ReferenceData.DeletedSongId,
			};

			var songs = await target.GetSongs(songIds, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedSongs = new[]
			{
				referenceData.SongWithOptionalPropertiesFilled1,
				referenceData.DeletedSong,
			};

			songs.Should().BeEquivalentTo(expectedSongs, x => x.WithStrictOrdering().IgnoringCyclicReferences());
		}

		[TestMethod]
		public async Task GetSongs_ListContainsUnknownSongs_ReturnsOnlyKnownSongs()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var songIds = new[]
			{
				ReferenceData.SongWithOptionalPropertiesFilledId1,
				ReferenceData.SongWithOptionalPropertiesMissingId,
				ReferenceData.NextSongId,
			};

			var songs = await target.GetSongs(songIds, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedSongs = new[]
			{
				referenceData.SongWithOptionalPropertiesFilled1,
				referenceData.SongWithOptionalPropertiesMissing,
			};

			songs.Should().BeEquivalentTo(expectedSongs, x => x.WithStrictOrdering().IgnoringCyclicReferences());
		}

		[TestMethod]
		public async Task UpdateSong_IfTreeTitleWasChanged_UpdatesSongDataCorrectly()
		{
			// Arrange

			var oldSongFilePath = Path.Combine(LibraryStorageRoot, "Belarusian", "Neuro Dubel", "2010 - Афтары правды (CD 1)", "01 - Про женщин.mp3");
			File.Exists(oldSongFilePath).Should().BeTrue();

			var updatedSong = await GetSong(ReferenceData.SongWithOptionalPropertiesFilledId1);

			var target = CreateTestTarget();

			// Act

			void UpdateSong(SongModel song)
			{
				updatedSong.TreeTitle = "11 - Дети Галактики.mp3";
			}

			await target.UpdateSong(updatedSong, UpdateSong, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedSong = referenceData.SongWithOptionalPropertiesFilled1;
			expectedSong.TreeTitle = "11 - Дети Галактики.mp3";
			expectedSong.ContentUri = "Belarusian/Neuro Dubel/2010 - Афтары правды (CD 1)/11 - Дети Галактики.mp3".ToContentUri(LibraryStorageRoot);

			updatedSong.Should().BeEquivalentTo(expectedSong, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var songFromRepository = await GetSong(ReferenceData.SongWithOptionalPropertiesFilledId1);
			songFromRepository.Should().BeEquivalentTo(expectedSong, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			File.Exists(oldSongFilePath).Should().BeFalse();

			var newSongFilePath = Path.Combine(LibraryStorageRoot, "Belarusian", "Neuro Dubel", "2010 - Афтары правды (CD 1)", "11 - Дети Галактики.mp3");
			File.Exists(newSongFilePath).Should().BeTrue();

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task UpdateSong_IfTagsRelatedDataWasChanged_UpdatesSongDataCorrectly()
		{
			// Arrange

			var updatedSong = await GetSong(ReferenceData.SongWithOptionalPropertiesFilledId1);

			var target = CreateTestTarget();

			// Act

			var newArtist = await GetArtist(ReferenceData.Artist1Id);
			var newGenre = await GetGenre(ReferenceData.Genre2Id);

			void UpdateSong(SongModel song)
			{
				updatedSong.TrackNumber = 17;
				updatedSong.Title = "Дети Галактики";
				updatedSong.Artist = newArtist;
				updatedSong.Genre = newGenre;
			}

			await target.UpdateSong(updatedSong, UpdateSong, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedSong = referenceData.SongWithOptionalPropertiesFilled1;
			expectedSong.TrackNumber = 17;
			expectedSong.Title = "Дети Галактики";
			expectedSong.Artist = referenceData.Artist1;
			expectedSong.Genre = referenceData.Genre2;
			expectedSong.Size = 405604;
			expectedSong.Checksum = 3102138277;

			updatedSong.Should().BeEquivalentTo(expectedSong, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var songFromRepository = await GetSong(ReferenceData.SongWithOptionalPropertiesFilledId1);
			songFromRepository.Should().BeEquivalentTo(expectedSong, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var songFilePath = Path.Combine(LibraryStorageRoot, "Belarusian", "Neuro Dubel", "2010 - Афтары правды (CD 1)", "01 - Про женщин.mp3");
			var fileInfo = new FileInfo(songFilePath);
			fileInfo.Length.Should().Be(405604);

			var expectedTagData = new SongTagData
			{
				Artist = "Guano Apes",
				Album = "Афтары правды",
				Year = 2010,
				Genre = "Alternative Rock",
				Track = 17,
				Title = "Дети Галактики",
			};

			var songTagger = GetService<ISongTagger>();
			var tagData = songTagger.GetTagData(songFilePath);
			tagData.Should().BeEquivalentTo(expectedTagData, x => x.WithStrictOrdering());

			await CheckLibraryConsistency(typeof(BadTrackNumbersInconsistency), typeof(MultipleDiscGenresInconsistency));
		}

		[TestMethod]
		public async Task UpdateSong_ForDeletedSong_UpdatesSongDataCorrectly()
		{
			// Arrange

			var updatedSong = await GetSong(ReferenceData.DeletedSongId);

			var target = CreateTestTarget();

			// Act

			var newArtist = await GetArtist(ReferenceData.Artist1Id);
			var newGenre = await GetGenre(ReferenceData.Genre2Id);

			void UpdateSong(SongModel song)
			{
				updatedSong.TrackNumber = 17;
				updatedSong.TreeTitle = "11 - Дети Галактики.mp3";
				updatedSong.Title = "Дети Галактики";
				updatedSong.Artist = newArtist;
				updatedSong.Genre = newGenre;
				updatedSong.DeleteComment = "New Delete Comment";
			}

			await target.UpdateSong(updatedSong, UpdateSong, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedSong = referenceData.DeletedSong;
			expectedSong.TrackNumber = 17;
			expectedSong.TreeTitle = "11 - Дети Галактики.mp3";
			expectedSong.Title = "Дети Галактики";
			expectedSong.Artist = referenceData.Artist1;
			expectedSong.Genre = referenceData.Genre2;
			expectedSong.DeleteComment = "New Delete Comment";

			updatedSong.Should().BeEquivalentTo(expectedSong, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var songFromRepository = await GetSong(ReferenceData.DeletedSongId);
			songFromRepository.Should().BeEquivalentTo(expectedSong, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			await CheckLibraryConsistency(typeof(BadTrackNumbersInconsistency), typeof(MultipleDiscGenresInconsistency));
		}

		[TestMethod]
		public async Task AddSongPlayback_ForSongWithoutPlaybacks_AddsPlaybackCorrectly()
		{
			// Arrange

			var song = await GetSong(ReferenceData.SongWithOptionalPropertiesMissingId);
			song.PlaybacksCount.Should().Be(0);

			var target = CreateTestTarget();

			// Act

			await target.AddSongPlayback(song, new DateTimeOffset(2021, 07, 04, 09, 27, 12, TimeSpan.FromHours(3)), CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedSong = referenceData.SongWithOptionalPropertiesMissing;
			expectedSong.AddPlayback(new DateTimeOffset(2021, 07, 04, 09, 27, 12, TimeSpan.FromHours(3)));

			song.Should().BeEquivalentTo(expectedSong, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var expectedPlaybacks = new[]
			{
				new PlaybackModel { Id = ReferenceData.NextPlaybackId, PlaybackTime = new DateTimeOffset(2021, 07, 04, 09, 27, 12, TimeSpan.FromHours(3)) },
			};

			var songFromRepository = await GetSongWithPlaybacks(ReferenceData.SongWithOptionalPropertiesMissingId);
			songFromRepository.Playbacks.Should().BeEquivalentTo(expectedPlaybacks, x => x.WithStrictOrdering());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task AddSongPlayback_ForSongWithPlaybacks_AddsPlaybackCorrectly()
		{
			// Arrange

			var song = await GetSong(ReferenceData.SongWithOptionalPropertiesFilledId1);
			song.PlaybacksCount.Should().Be(2);

			var target = CreateTestTarget();

			// Act

			await target.AddSongPlayback(song, new DateTimeOffset(2021, 07, 04, 09, 27, 12, TimeSpan.FromHours(3)), CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedSong = referenceData.SongWithOptionalPropertiesFilled1;
			expectedSong.AddPlayback(new DateTimeOffset(2021, 07, 04, 09, 27, 12, TimeSpan.FromHours(3)));

			song.Should().BeEquivalentTo(expectedSong, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var expectedPlaybacks = new[]
			{
				new PlaybackModel { Id = new ItemId("1"), PlaybackTime = DateTimeOffset.Parse("2021-03-19 13:35:02.2626013+03:00", CultureInfo.InvariantCulture) },
				new PlaybackModel { Id = new ItemId("4"), PlaybackTime = DateTimeOffset.Parse("2021-04-03 10:33:53.3517221+03:00", CultureInfo.InvariantCulture) },
				new PlaybackModel { Id = ReferenceData.NextPlaybackId, PlaybackTime = new DateTimeOffset(2021, 07, 04, 09, 27, 12, TimeSpan.FromHours(3)) },
			};

			var songFromRepository = await GetSongWithPlaybacks(ReferenceData.SongWithOptionalPropertiesFilledId1);
			songFromRepository.Playbacks.Should().BeEquivalentTo(expectedPlaybacks, x => x.WithStrictOrdering());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task DeleteSong_DeletesSongCorrectly()
		{
			// Arrange

			var deleteDate = new DateTimeOffset(2021, 07, 04, 11, 22, 15, TimeSpan.FromHours(3));
			var target = CreateTestTarget(StubClock(deleteDate));

			var songFilePath = Path.Combine(LibraryStorageRoot, "Belarusian", "Neuro Dubel", "2010 - Афтары правды (CD 1)", "02 - Про жизнь дяди Саши.mp3");
			File.Exists(songFilePath).Should().BeTrue();

			var song = await GetSong(ReferenceData.SongWithOptionalPropertiesFilledId2);

			// Act

			await target.DeleteSong(song, "Some Delete Comment", CancellationToken.None);

			// Assert

			var expectedSongWithoutPlaybacks = GetReferenceData(fillSongPlaybacks: false).SongWithOptionalPropertiesFilled2;
			expectedSongWithoutPlaybacks.MarkAsDeleted(deleteDate, "Some Delete Comment");
			song.Should().BeEquivalentTo(expectedSongWithoutPlaybacks, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var expectedSongWithPlaybacks = GetReferenceData(fillSongPlaybacks: true).SongWithOptionalPropertiesFilled2;
			expectedSongWithPlaybacks.MarkAsDeleted(deleteDate, "Some Delete Comment");
			var songFromRepository = await GetSongWithPlaybacks(ReferenceData.SongWithOptionalPropertiesFilledId2);
			songFromRepository.Should().BeEquivalentTo(expectedSongWithPlaybacks, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			File.Exists(songFilePath).Should().BeFalse();

			await CheckLibraryConsistency();
		}

		private async Task<ArtistModel> GetArtist(ItemId artistId)
		{
			var artistService = GetService<IArtistsService>();
			var allArtists = await artistService.GetAllArtists(CancellationToken.None);
			return allArtists.Single(x => x.Id == artistId);
		}

		private async Task<GenreModel> GetGenre(ItemId genreId)
		{
			var genreService = GetService<IGenresService>();
			var allGenres = await genreService.GetAllGenres(CancellationToken.None);
			return allGenres.Single(x => x.Id == genreId);
		}

		private async Task<SongModel> GetSong(ItemId songId)
		{
			var discService = GetService<IDiscsService>();
			var allDiscs = await discService.GetAllDiscs(CancellationToken.None);
			return allDiscs
				.Single(d => d.AllSongs.Any(s => s.Id == songId))
				.AllSongs.Single(s => s.Id == songId);
		}

		private async Task<SongModel> GetSongWithPlaybacks(ItemId songId)
		{
			var libraryRepository = GetService<IDiscLibraryRepository>();
			var libraryWithPlaybacks = await libraryRepository.ReadDiscLibraryWithPlaybacks(CancellationToken.None);

			return libraryWithPlaybacks.TryGetSongs(new[] { songId }).Single();
		}
	}
}
