using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Extensions;
using MusicLibrary.Services.IntegrationTests.Data;
using MusicLibrary.Services.IntegrationTests.Extensions;
using MusicLibrary.Services.Interfaces;
using MusicLibrary.Services.Tagging;

namespace MusicLibrary.Services.IntegrationTests
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
				Disc = await GetDisc(ReferenceData.NormalDiscId),
				Title = "Some New Song",
				TreeTitle = "02 - Some New Song.mp3",
				TrackNumber = 2,
				Artist = await GetArtist(ReferenceData.Artist1Id),
				Genre = await GetGenre(ReferenceData.Genre1Id),
				Duration = TimeSpan.FromMilliseconds(12345),
				Rating = RatingModel.R7,
				BitRate = 54321,
			};

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
				Disc = expectedDisc,
				Title = "Some New Song",
				TreeTitle = "02 - Some New Song.mp3",
				TrackNumber = 2,
				Artist = referenceData.Artist1,
				Genre = referenceData.Genre1,
				Duration = TimeSpan.FromMilliseconds(12345),
				Rating = RatingModel.R7,
				BitRate = 54321,
				Size = 416101,
				Checksum = 1450589396,
				ContentUri = "Foreign/Guano Apes/2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)/02 - Some New Song.mp3".ToContentUri(LibraryStorageRoot),
			};

			expectedDisc.AllSongs = expectedDisc.AllSongs.Concat(new[] { expectedSong }).ToList();

			var updatedDisc = await GetDisc(ReferenceData.NormalDiscId);
			updatedDisc.Should().BeEquivalentTo(expectedDisc, x => x.IgnoringCyclicReferences());

			var songFilePath = Path.Combine(LibraryStorageRoot, "Foreign", "Guano Apes", "2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)", "02 - Some New Song.mp3");
			var fileInfo = new FileInfo(songFilePath);
			fileInfo.Exists.Should().BeTrue();
			fileInfo.Length.Should().Be(416101);

			var expectedTagData = new SongTagData
			{
				Artist = "Guano Apes",
				Album = "Planet Of The Apes - Best Of Guano Apes",
				Year = 2004,
				Genre = "Alternative Rock",
				Track = 2,
				Title = "Some New Song",
			};

			var songTagger = GetService<ISongTagger>();
			var tagData = songTagger.GetTagData(songFilePath);
			tagData.Should().BeEquivalentTo(expectedTagData);
		}

		[TestMethod]
		public async Task CreateSong_ForSongWithAllOptionalPropertiesNotFilled_CreatesSongSuccessfully()
		{
			// Arrange

			var newSong = new SongModel
			{
				Disc = await GetDisc(ReferenceData.NormalDiscId),
				Title = "Some New Song",
				TreeTitle = "02 - Some New Song.mp3",
				Duration = TimeSpan.FromMilliseconds(12345),
				BitRate = 54321,
			};

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
				Disc = expectedDisc,
				Title = "Some New Song",
				TreeTitle = "02 - Some New Song.mp3",
				Duration = TimeSpan.FromMilliseconds(12345),
				BitRate = 54321,
				Size = 416008,
				Checksum = 3126574971,
				ContentUri = "Foreign/Guano Apes/2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)/02 - Some New Song.mp3".ToContentUri(LibraryStorageRoot),
			};

			expectedDisc.AllSongs = expectedDisc.AllSongs.Concat(new[] { expectedSong }).ToList();

			var updatedDisc = await GetDisc(ReferenceData.NormalDiscId);
			updatedDisc.Should().BeEquivalentTo(expectedDisc, x => x.IgnoringCyclicReferences());

			var songFilePath = Path.Combine(LibraryStorageRoot, "Foreign", "Guano Apes", "2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)", "02 - Some New Song.mp3");
			var fileInfo = new FileInfo(songFilePath);
			fileInfo.Exists.Should().BeTrue();
			fileInfo.Length.Should().Be(416008);

			var expectedTagData = new SongTagData
			{
				Album = "Planet Of The Apes - Best Of Guano Apes",
				Year = 2004,
				Title = "Some New Song",
			};

			var songTagger = GetService<ISongTagger>();
			var tagData = songTagger.GetTagData(songFilePath);
			tagData.Should().BeEquivalentTo(expectedTagData);
		}

		[TestMethod]
		public async Task CreateSong_ForSongContentWithFilledTags_ClearsExistingTags()
		{
			// Arrange

			var newSong = new SongModel
			{
				Disc = await GetDisc(ReferenceData.NormalDiscId),
				Title = "Some New Song",
				TreeTitle = "02 - Some New Song.mp3",
				Duration = TimeSpan.FromMilliseconds(12345),
				BitRate = 54321,
			};

			var target = CreateTestTarget();

			// Act

			await using var songContent = File.OpenRead("ContentForAdding/New Song With Tags.mp3");
			await target.CreateSong(newSong, songContent, CancellationToken.None);

			// Assert

			var songFilePath = Path.Combine(LibraryStorageRoot, "Foreign", "Guano Apes", "2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)", "02 - Some New Song.mp3");

			var expectedTagData = new SongTagData
			{
				Album = "Planet Of The Apes - Best Of Guano Apes",
				Year = 2004,
				Title = "Some New Song",
			};

			var songTagger = GetService<ISongTagger>();
			var tagData = songTagger.GetTagData(songFilePath);
			tagData.Should().BeEquivalentTo(expectedTagData);
		}

		[TestMethod]
		public async Task GetSongs_ForExistingActiveSongIds_ReturnsSongsCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var songIds = new[]
			{
				ReferenceData.SongWithOptionalPropertiesFilledId,
				ReferenceData.SongFromNullDiscId,
			};

			var songs = await target.GetSongs(songIds, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedSongs = new[]
			{
				referenceData.SongWithOptionalPropertiesFilled,
				referenceData.SongFromNullDisc,
			};

			songs.OrderBy(x => x.Id.ToInt32()).Should().BeEquivalentTo(expectedSongs, x => x.IgnoringCyclicReferences());
		}

		[TestMethod]
		public async Task GetSongs_ListContainsDeletedSongs_ReturnsDeletedSongs()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var songIds = new[]
			{
				ReferenceData.SongWithOptionalPropertiesFilledId,
				ReferenceData.DeletedSongId,
			};

			var songs = await target.GetSongs(songIds, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedSongs = new[]
			{
				referenceData.SongWithOptionalPropertiesFilled,
				referenceData.DeletedSong,
			};

			songs.OrderBy(x => x.Id.ToInt32()).Should().BeEquivalentTo(expectedSongs, x => x.IgnoringCyclicReferences());
		}

		[TestMethod]
		public async Task GetSongs_ListContainsUnknownSongs_ReturnsOnlyKnownSongs()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var songIds = new[]
			{
				ReferenceData.SongWithOptionalPropertiesFilledId,
				ReferenceData.SongWithOptionalPropertiesMissingId,
				ReferenceData.NextSongId,
			};

			var songs = await target.GetSongs(songIds, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedSongs = new[]
			{
				referenceData.SongWithOptionalPropertiesFilled,
				referenceData.SongWithOptionalPropertiesMissing,
			};

			songs.OrderBy(x => x.Id.ToInt32()).Should().BeEquivalentTo(expectedSongs, x => x.IgnoringCyclicReferences());
		}

		[TestMethod]
		public async Task UpdateSong_IfTreeTitleWasChanged_UpdatesSongDataCorrectly()
		{
			// Arrange

			var oldSongFilePath = Path.Combine(LibraryStorageRoot, "Foreign", "Guano Apes", "2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)", "01 - Break The Line.mp3");
			File.Exists(oldSongFilePath).Should().BeTrue();

			var updatedSong = await GetSong(ReferenceData.SongWithOptionalPropertiesFilledId);

			var target = CreateTestTarget();

			// Act

			updatedSong.TreeTitle = "New Tree Title.mp3";

			await target.UpdateSong(updatedSong, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedSong = referenceData.SongWithOptionalPropertiesFilled;
			expectedSong.TreeTitle = "New Tree Title.mp3";
			expectedSong.ContentUri = "Foreign/Guano Apes/2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)/New Tree Title.mp3".ToContentUri(LibraryStorageRoot);

			updatedSong.Should().BeEquivalentTo(expectedSong, x => x.IgnoringCyclicReferences());

			var songFromRepository = await GetSong(ReferenceData.SongWithOptionalPropertiesFilledId);
			songFromRepository.Should().BeEquivalentTo(expectedSong, x => x.IgnoringCyclicReferences());

			File.Exists(oldSongFilePath).Should().BeFalse();

			var newSongFilePath = Path.Combine(LibraryStorageRoot, "Foreign", "Guano Apes", "2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)", "New Tree Title.mp3");
			File.Exists(newSongFilePath).Should().BeTrue();
		}

		[TestMethod]
		public async Task UpdateSong_IfTagsRelatedDataWasChanged_UpdatesSongDataCorrectly()
		{
			// Arrange

			var updatedSong = await GetSong(ReferenceData.SongWithOptionalPropertiesFilledId);

			var target = CreateTestTarget();

			// Act

			updatedSong.TrackNumber = 17;
			updatedSong.Title = "New Song Title";
			updatedSong.Artist = await GetArtist(ReferenceData.Artist2Id);
			updatedSong.Genre = await GetGenre(ReferenceData.Genre2Id);

			await target.UpdateSong(updatedSong, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedSong = referenceData.SongWithOptionalPropertiesFilled;
			expectedSong.TrackNumber = 17;
			expectedSong.Title = "New Song Title";
			expectedSong.Artist = referenceData.Artist2;
			expectedSong.Genre = referenceData.Genre2;
			expectedSong.Size = 405634;
			expectedSong.Checksum = 2283678480;

			updatedSong.Should().BeEquivalentTo(expectedSong, x => x.IgnoringCyclicReferences());

			var songFromRepository = await GetSong(ReferenceData.SongWithOptionalPropertiesFilledId);
			songFromRepository.Should().BeEquivalentTo(expectedSong, x => x.IgnoringCyclicReferences());

			var songFilePath = Path.Combine(LibraryStorageRoot, "Foreign", "Guano Apes", "2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)", "01 - Break The Line.mp3");
			var fileInfo = new FileInfo(songFilePath);
			fileInfo.Length.Should().Be(405634);

			var expectedTagData = new SongTagData
			{
				Artist = "Neuro Dubel",
				Album = "Planet Of The Apes - Best Of Guano Apes",
				Year = 2004,
				Genre = "Rock",
				Track = 17,
				Title = "New Song Title",
			};

			var songTagger = GetService<ISongTagger>();
			var tagData = songTagger.GetTagData(songFilePath);
			tagData.Should().BeEquivalentTo(expectedTagData);
		}

		[TestMethod]
		public async Task AddSongPlayback_ForSongWithoutPlaybacks_AddsPlaybackCorrectly()
		{
			// Arrange

			var song = await GetSongWithPlaybacks(ReferenceData.SongWithOptionalPropertiesMissingId);
			song.PlaybacksCount.Should().Be(0);

			var target = CreateTestTarget();

			// Act

			await target.AddSongPlayback(song, new DateTimeOffset(2021, 07, 04, 09, 27, 12, TimeSpan.FromHours(3)), CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData(fillSongPlaybacks: true);
			var expectedSong = referenceData.SongWithOptionalPropertiesMissing;
			expectedSong.PlaybacksCount = 1;
			expectedSong.LastPlaybackTime = new DateTimeOffset(2021, 07, 04, 09, 27, 12, TimeSpan.FromHours(3));
			expectedSong.Playbacks = new List<PlaybackModel>
			{
				new()
				{
					Id = ReferenceData.NextPlaybackId,
					PlaybackTime = new DateTimeOffset(2021, 07, 04, 09, 27, 12, TimeSpan.FromHours(3)),
				},
			};

			// SongModel.AddPlayback() does not update Playbacks collection, that is why we exclude it for this check.
			song.Should().BeEquivalentTo(expectedSong, x => x.Excluding(y => y.Playbacks).IgnoringCyclicReferences());

			var songFromRepository = await GetSongWithPlaybacks(ReferenceData.SongWithOptionalPropertiesMissingId);
			songFromRepository.Should().BeEquivalentTo(expectedSong, x => x.IgnoringCyclicReferences());
		}

		[TestMethod]
		public async Task AddSongPlayback_ForSongWithPlaybacks_AddsPlaybackCorrectly()
		{
			// Arrange

			var song = await GetSongWithPlaybacks(ReferenceData.SongWithOptionalPropertiesFilledId);
			song.PlaybacksCount.Should().Be(2);

			var target = CreateTestTarget();

			// Act

			await target.AddSongPlayback(song, new DateTimeOffset(2021, 07, 04, 09, 27, 12, TimeSpan.FromHours(3)), CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData(fillSongPlaybacks: true);
			var expectedSong = referenceData.SongWithOptionalPropertiesFilled;
			expectedSong.PlaybacksCount = 3;
			expectedSong.LastPlaybackTime = new DateTimeOffset(2021, 07, 04, 09, 27, 12, TimeSpan.FromHours(3));
			expectedSong.Playbacks = expectedSong.Playbacks.Concat(new PlaybackModel[]
			{
				new()
				{
					Id = ReferenceData.NextPlaybackId,
					PlaybackTime = new DateTimeOffset(2021, 07, 04, 09, 27, 12, TimeSpan.FromHours(3)),
				},
			}).ToList();

			// SongModel.AddPlayback() does not update Playbacks collection, that is why we exclude it for this check.
			song.Should().BeEquivalentTo(expectedSong, x => x.Excluding(y => y.Playbacks).IgnoringCyclicReferences());

			var songFromRepository = await GetSongWithPlaybacks(ReferenceData.SongWithOptionalPropertiesFilledId);
			songFromRepository.Should().BeEquivalentTo(expectedSong, x => x.IgnoringCyclicReferences());
		}

		[TestMethod]
		public async Task DeleteSong_DeletesSongCorrectly()
		{
			// Arrange

			var songFilePath = Path.Combine(LibraryStorageRoot, "Foreign", "Guano Apes", "2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)", "01 - Break The Line.mp3");
			File.Exists(songFilePath).Should().BeTrue();

			var song = await GetSong(ReferenceData.SongWithOptionalPropertiesFilledId);

			var target = CreateTestTarget();

			// Act

			await target.DeleteSong(song, new DateTimeOffset(2021, 07, 04, 11, 22, 15, TimeSpan.FromHours(3)), CancellationToken.None);

			// Assert

			var expectedSongWithoutPlaybacks = GetReferenceData(fillSongPlaybacks: false).SongWithOptionalPropertiesFilled;
			expectedSongWithoutPlaybacks.DeleteDate = new DateTimeOffset(2021, 07, 04, 11, 22, 15, TimeSpan.FromHours(3));
			expectedSongWithoutPlaybacks.BitRate = null;
			expectedSongWithoutPlaybacks.Size = null;
			expectedSongWithoutPlaybacks.Checksum = null;
			expectedSongWithoutPlaybacks.ContentUri = null;

			song.Should().BeEquivalentTo(expectedSongWithoutPlaybacks, x => x.IgnoringCyclicReferences());

			var expectedSongWithPlaybacks = GetReferenceData(fillSongPlaybacks: true).SongWithOptionalPropertiesFilled;
			expectedSongWithPlaybacks.DeleteDate = new DateTimeOffset(2021, 07, 04, 11, 22, 15, TimeSpan.FromHours(3));
			expectedSongWithPlaybacks.BitRate = null;
			expectedSongWithPlaybacks.Size = null;
			expectedSongWithPlaybacks.Checksum = null;
			expectedSongWithPlaybacks.ContentUri = null;

			var songFromRepository = await GetSongWithPlaybacks(ReferenceData.SongWithOptionalPropertiesFilledId);
			songFromRepository.Should().BeEquivalentTo(expectedSongWithPlaybacks, x => x.IgnoringCyclicReferences());

			File.Exists(songFilePath).Should().BeFalse();
		}

		private async Task<DiscModel> GetDisc(ItemId discId)
		{
			var discService = GetService<IDiscsService>();
			var allDiscs = await discService.GetAllDiscs(CancellationToken.None);
			return allDiscs.Single(x => x.Id == discId);
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
			var songService = GetService<ISongsService>();
			return await songService.GetSongWithPlaybacks(songId, CancellationToken.None);
		}
	}
}
