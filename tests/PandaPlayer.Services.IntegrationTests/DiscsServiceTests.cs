using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PandaPlayer.Core.Models;
using PandaPlayer.Dal.LocalDb.Inconsistencies.StorageInconsistencies;
using PandaPlayer.Services.Diagnostic.Inconsistencies.DiscInconsistencies;
using PandaPlayer.Services.IntegrationTests.Data;
using PandaPlayer.Services.IntegrationTests.Extensions;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Services.Tagging;

namespace PandaPlayer.Services.IntegrationTests
{
	[TestClass]
	public class DiscsServiceTests : BasicServiceTests<IDiscsService>
	{
		[TestMethod]
		public async Task CreateDisc_ForDiscWithAllOptionalPropertiesFilled_CreatesDiscSuccessfully()
		{
			async Task<DiscModel> CreateDiscData(bool fillId, bool addSong)
			{
				var disc = new DiscModel
				{
					Id = fillId ? ReferenceData.NextDiscId : null,
					Folder = await GetFolder(ReferenceData.ArtistFolderId),
					Year = 1994,
					Title = "Титаник Live (CD 1)",
					TreeTitle = "1994 - Титаник Live (CD 1)",
					AlbumTitle = "Титаник",
				};

				if (addSong)
				{
					// Songs are not added by CreateDisc method.
					// However we fill songs list so that CreateDisc is tested in more real-life conditions.
					disc.AllSongs = new[]
					{
						new SongModel
						{
							Id = ReferenceData.NextSongId,
							Disc = disc,
							Title = "Интродукция",
							TreeTitle = "01 - Интродукция.mp3",
							Duration = new TimeSpan(0, 3, 12),
							BitRate = 12345,
							Size = 67890,
							Checksum = 54321,
						},
					};
				}
				else
				{
					disc.AllSongs = new List<SongModel>();
				}

				return disc;
			}

			await TestCaseForCreateDisc(CreateDiscData, Path.Combine("Belarusian", "Neuro Dubel", "1994 - Титаник Live (CD 1)"));
		}

		[TestMethod]
		public async Task CreateDisc_ForDiscWithAllOptionalPropertiesNotFilled_CreatesDiscSuccessfully()
		{
			async Task<DiscModel> CreateDiscData(bool fillId, bool addSong)
			{
				var disc = new DiscModel
				{
					Id = fillId ? ReferenceData.NextDiscId : null,
					Folder = await GetFolder(ReferenceData.ArtistFolderId),
					Title = "Титаник Live (CD 1)",
					TreeTitle = "1994 - Титаник Live (CD 1)",
				};

				if (addSong)
				{
					// Songs are not added by CreateDisc method.
					// However we fill songs list so that CreateDisc is tested in more real-life conditions.
					disc.AllSongs = new[]
					{
						new SongModel
						{
							Id = ReferenceData.NextSongId,
							Title = "Интродукция",
							TreeTitle = "01 - Интродукция.mp3",
							Duration = new TimeSpan(0, 3, 12),
							BitRate = 12345,
							Size = 67890,
							Checksum = 54321,
						},
					};
				}
				else
				{
					disc.AllSongs = new List<SongModel>();
				}

				return disc;
			}

			await TestCaseForCreateDisc(CreateDiscData, Path.Combine("Belarusian", "Neuro Dubel", "1994 - Титаник Live (CD 1)"));
		}

		[TestMethod]
		public async Task CreateDisc_ForFirstDiscInFolder_CreatesDiscSuccessfully()
		{
			async Task<DiscModel> CreateDiscData(bool fillId, bool addSong)
			{
				return new()
				{
					Id = fillId ? ReferenceData.NextDiscId : null,
					Folder = await GetFolder(ReferenceData.EmptyFolderId),
					Title = "Титаник Live (CD 1)",
					TreeTitle = "1994 - Титаник Live (CD 1)",
					AllSongs = new List<SongModel>(),
				};
			}

			await TestCaseForCreateDisc(CreateDiscData, Path.Combine("Belarusian", "Neuro Dubel", "Empty Folder", "1994 - Титаник Live (CD 1)"));
		}

		// discDataFactory(bool fillId, bool addSong)
		private async Task TestCaseForCreateDisc(Func<bool, bool, Task<DiscModel>> discDataFactory, string relativeDiscDirectoryPath)
		{
			// Arrange

			var newDisc = await discDataFactory(false, true);

			var target = CreateTestTarget();

			// Act

			await target.CreateDisc(newDisc, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedDisc = await discDataFactory(true, true);

			newDisc.Should().BeEquivalentTo(expectedDisc, x => x.IgnoringCyclicReferences());

			var expectedDiscs = new[]
			{
				referenceData.NormalDisc,
				referenceData.DiscWithMissingFields,
				referenceData.DeletedDisc,
				await discDataFactory(true, false),
			};

			var allDiscs = await target.GetAllDiscs(CancellationToken.None);
			allDiscs.Should().BeEquivalentTo(expectedDiscs, x => x.IgnoringCyclicReferences());

			var discDirectoryPath = Path.Combine(LibraryStorageRoot, relativeDiscDirectoryPath);
			Directory.Exists(discDirectoryPath).Should().BeTrue();

			// This test creates empty disc which is considered as deleted (no active songs).
			// Thus, disc directory is not expected in library storage.
			await CheckLibraryConsistency(typeof(UnexpectedFolderInconsistency));
		}

		[TestMethod]
		public async Task GetAllDiscs_ReturnsDiscsCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var discs = await target.GetAllDiscs(CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedDiscs = new[]
			{
				referenceData.NormalDisc,
				referenceData.DiscWithMissingFields,
				referenceData.DeletedDisc,
			};

			discs.Should().BeEquivalentTo(expectedDiscs, x => x.IgnoringCyclicReferences());
		}

		[TestMethod]
		public async Task UpdateDisc_IfTreeTitleWasChanged_UpdatesStorageDataCorrectly()
		{
			// Arrange

			var oldDiscDirectoryPath = Path.Combine(LibraryStorageRoot, "Belarusian", "Neuro Dubel", "2010 - Афтары правды (CD 1)");
			Directory.Exists(oldDiscDirectoryPath).Should().BeTrue();

			var target = CreateTestTarget();

			var disc = await GetDisc(ReferenceData.NormalDiscId);

			// Act

			disc.TreeTitle = "1998 - Охотник и сайгак";

			await target.UpdateDisc(disc, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedDisc = referenceData.NormalDisc;
			expectedDisc.TreeTitle = "1998 - Охотник и сайгак";
			var activeSongs = expectedDisc.ActiveSongs.ToList();
			activeSongs[0].ContentUri = "Belarusian/Neuro Dubel/1998 - Охотник и сайгак/01 - Про женщин.mp3".ToContentUri(LibraryStorageRoot);
			activeSongs[1].ContentUri = "Belarusian/Neuro Dubel/1998 - Охотник и сайгак/02 - Про жизнь дяди Саши.mp3".ToContentUri(LibraryStorageRoot);
			expectedDisc.Images.Single().ContentUri = "Belarusian/Neuro Dubel/1998 - Охотник и сайгак/cover.jpg".ToContentUri(LibraryStorageRoot);

			disc.Should().BeEquivalentTo(expectedDisc, x => x.IgnoringCyclicReferences());

			var discFromRepository = await GetDisc(ReferenceData.NormalDiscId);
			discFromRepository.Should().BeEquivalentTo(expectedDisc, x => x.IgnoringCyclicReferences());

			Directory.Exists(oldDiscDirectoryPath).Should().BeFalse();

			var newDiscDirectoryPath = Path.Combine(LibraryStorageRoot, "Belarusian", "Neuro Dubel", "1998 - Охотник и сайгак");
			Directory.Exists(newDiscDirectoryPath).Should().BeTrue();

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task UpdateDisc_IfTagsRelatedDataWasChanged_UpdatesDiscCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			var disc = await GetDisc(ReferenceData.NormalDiscId);

			// Act

			disc.Year = 1998;
			disc.AlbumTitle = "Охотник и сайгак";

			await target.UpdateDisc(disc, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedDisc = referenceData.NormalDisc;
			expectedDisc.Year = 1998;
			expectedDisc.AlbumTitle = "Охотник и сайгак";
			var activeSongs = expectedDisc.ActiveSongs.ToList();
			activeSongs[0].Size = 405588;
			activeSongs[0].Checksum = 1321629719;
			activeSongs[1].Size = 404561;
			activeSongs[1].Checksum = 3202969334;

			disc.Should().BeEquivalentTo(expectedDisc, x => x.IgnoringCyclicReferences());

			var discFromRepository = await GetDisc(ReferenceData.NormalDiscId);
			discFromRepository.Should().BeEquivalentTo(expectedDisc, x => x.IgnoringCyclicReferences());

			var songTagger = GetService<ISongTagger>();

			var tagData1 = songTagger.GetTagData(Path.Combine(LibraryStorageRoot, "Belarusian", "Neuro Dubel", "2010 - Афтары правды (CD 1)", "01 - Про женщин.mp3"));
			tagData1.Year.Should().Be(1998);
			tagData1.Album.Should().Be("Охотник и сайгак");

			var tagData2 = songTagger.GetTagData(Path.Combine(LibraryStorageRoot, "Belarusian", "Neuro Dubel", "2010 - Афтары правды (CD 1)", "02 - Про жизнь дяди Саши.mp3"));
			tagData2.Year.Should().Be(1998);
			tagData2.Album.Should().Be("Охотник и сайгак");

			var updatedDisc = await GetDisc(ReferenceData.NormalDiscId);
			updatedDisc.Should().BeEquivalentTo(disc, x => x.IgnoringCyclicReferences());

			await CheckLibraryConsistency(typeof(SuspiciousAlbumTitleInconsistency));
		}

		[TestMethod]
		public async Task SetDiscCoverImage_ForDiscWithoutCoverImage_SetsCoverImageCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			var imageFilePath = Path.Combine(LibraryStorageRoot, "Belarusian", "Neuro Dubel", "Disc With Missing Fields (CD 1)", "cover.jpg");
			File.Exists(imageFilePath).Should().BeFalse();

			var discCoverImage = new DiscImageModel
			{
				Disc = await GetDisc(ReferenceData.DiscWithMissingFieldsId),
				TreeTitle = "cover.jpg",
				ImageType = DiscImageType.Cover,
			};

			// Act

			await using var imageContent = File.OpenRead("ContentForAdding/NewCover.jpg");
			await target.SetDiscCoverImage(discCoverImage, imageContent, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedDiscCoverImage = new DiscImageModel
			{
				Id = ReferenceData.NextDiscCoverImageId,
				Disc = referenceData.DiscWithMissingFields,
				TreeTitle = "cover.jpg",
				ImageType = DiscImageType.Cover,
				Size = 119957,
				Checksum = 1208131419,
				ContentUri = "Belarusian/Neuro Dubel/Disc With Missing Fields (CD 1)/cover.jpg".ToContentUri(LibraryStorageRoot),
			};

			referenceData.DiscWithMissingFields.Images = new[] { expectedDiscCoverImage };

			discCoverImage.Should().BeEquivalentTo(expectedDiscCoverImage, x => x.IgnoringCyclicReferences());

			var discFromRepository = await GetDisc(ReferenceData.DiscWithMissingFieldsId);
			discFromRepository.CoverImage.Should().BeEquivalentTo(expectedDiscCoverImage, x => x.IgnoringCyclicReferences());

			var fileInfo = new FileInfo(imageFilePath);
			fileInfo.Exists.Should().BeTrue();
			fileInfo.Length.Should().Be(119957);

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task SetDiscCoverImage_ForDiscWithExistingCoverImageOfSameType_SetsCoverImageCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			var imageFilePath = Path.Combine(LibraryStorageRoot, "Belarusian", "Neuro Dubel", "2010 - Афтары правды (CD 1)", "cover.jpg");
			File.Exists(imageFilePath).Should().BeTrue();

			var discCoverImage = new DiscImageModel
			{
				Disc = await GetDisc(ReferenceData.NormalDiscId),
				TreeTitle = "cover.jpg",
				ImageType = DiscImageType.Cover,
			};

			// Act

			await using var imageContent = File.OpenRead("ContentForAdding/NewCover.jpg");
			await target.SetDiscCoverImage(discCoverImage, imageContent, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedDiscCoverImage = new DiscImageModel
			{
				Id = ReferenceData.DiscCoverImageId,
				Disc = referenceData.NormalDisc,
				TreeTitle = "cover.jpg",
				ImageType = DiscImageType.Cover,
				Size = 119957,
				Checksum = 1208131419,
				ContentUri = "Belarusian/Neuro Dubel/2010 - Афтары правды (CD 1)/cover.jpg".ToContentUri(LibraryStorageRoot),
			};

			referenceData.NormalDisc.Images = new[] { expectedDiscCoverImage };

			discCoverImage.Should().BeEquivalentTo(expectedDiscCoverImage, x => x.IgnoringCyclicReferences());

			var discFromRepository = await GetDisc(ReferenceData.NormalDiscId);
			discFromRepository.CoverImage.Should().BeEquivalentTo(expectedDiscCoverImage, x => x.IgnoringCyclicReferences());

			var fileInfo = new FileInfo(imageFilePath);
			fileInfo.Exists.Should().BeTrue();
			fileInfo.Length.Should().Be(119957);

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task SetDiscCoverImage_ForDiscWithExistingCoverImageOfAnotherType_SetsCoverImageCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			var oldImageFilePath = Path.Combine(LibraryStorageRoot, "Belarusian", "Neuro Dubel", "2010 - Афтары правды (CD 1)", "cover.jpg");
			File.Exists(oldImageFilePath).Should().BeTrue();

			var discCoverImage = new DiscImageModel
			{
				Disc = await GetDisc(ReferenceData.NormalDiscId),
				TreeTitle = "cover.png",
				ImageType = DiscImageType.Cover,
			};

			// Act

			await using var imageContent = File.OpenRead("ContentForAdding/NewCover.png");
			await target.SetDiscCoverImage(discCoverImage, imageContent, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedDiscCoverImage = new DiscImageModel
			{
				Id = ReferenceData.DiscCoverImageId,
				Disc = referenceData.NormalDisc,
				TreeTitle = "cover.png",
				ImageType = DiscImageType.Cover,
				Size = 184257,
				Checksum = 1738836760,
				ContentUri = "Belarusian/Neuro Dubel/2010 - Афтары правды (CD 1)/cover.png".ToContentUri(LibraryStorageRoot),
			};

			referenceData.NormalDisc.Images = new[] { expectedDiscCoverImage };

			discCoverImage.Should().BeEquivalentTo(expectedDiscCoverImage, x => x.IgnoringCyclicReferences());

			var discFromRepository = await GetDisc(ReferenceData.NormalDiscId);
			discFromRepository.CoverImage.Should().BeEquivalentTo(expectedDiscCoverImage, x => x.IgnoringCyclicReferences());

			var imageFilePath = Path.Combine(LibraryStorageRoot, "Belarusian", "Neuro Dubel", "2010 - Афтары правды (CD 1)", "cover.png");
			var fileInfo = new FileInfo(imageFilePath);
			fileInfo.Exists.Should().BeTrue();
			fileInfo.Length.Should().Be(184257);

			File.Exists(oldImageFilePath).Should().BeFalse();

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task DeleteDisc_DeletesDiscSuccessfully()
		{
			// Arrange

			var discDirectoryPath = Path.Combine(LibraryStorageRoot, "Belarusian", "Neuro Dubel", "2010 - Афтары правды (CD 1)");
			Directory.Exists(discDirectoryPath).Should().BeTrue();

			var deleteDate = new DateTimeOffset(2021, 07, 03, 13, 25, 33, TimeSpan.FromHours(3));
			var target = CreateTestTarget(StubClock(deleteDate));

			// Act

			await target.DeleteDisc(ReferenceData.NormalDiscId, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedDisc = referenceData.NormalDisc;
			foreach (var song in expectedDisc.ActiveSongs)
			{
				song.DeleteDate = deleteDate;
				song.BitRate = null;
				song.Size = null;
				song.Checksum = null;
				song.ContentUri = null;
			}

			expectedDisc.Images = new List<DiscImageModel>();
			expectedDisc.AdviseGroup = null;
			expectedDisc.AdviseSetInfo = null;

			var discFromRepository = await GetDisc(ReferenceData.NormalDiscId);
			discFromRepository.Should().BeEquivalentTo(expectedDisc, x => x.IgnoringCyclicReferences());

			Directory.Exists(discDirectoryPath).Should().BeFalse();

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task DeleteDisc_IfDiscIsLastHolderOfAdviseGroup_DeletesAssignedAdviseGroup()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			await target.DeleteDisc(ReferenceData.NormalDiscId, CancellationToken.None);

			// Assert

			var discFromRepository = await GetDisc(ReferenceData.NormalDiscId);
			discFromRepository.AdviseGroup.Should().BeNull();

			var referenceData = GetReferenceData();
			var expectedAdviseGroups = new[]
			{
				referenceData.FolderAdviseGroup,
			};

			var adviseGroupService = GetService<IAdviseGroupService>();
			var adviseGroups = await adviseGroupService.GetAllAdviseGroups(CancellationToken.None);
			adviseGroups.Should().BeEquivalentTo(expectedAdviseGroups, x => x.WithStrictOrdering());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task DeleteDisc_IfDiscIsNotLastHolderOfAdviseGroup_DoesNotDeletesAssignedAdviseGroup()
		{
			// Arrange

			// Assigning one more reference to DiscAdviseGroup.
			await AssignAdviseGroupToFolder(ReferenceData.EmptyFolderId, ReferenceData.DiscAdviseGroupId);

			var target = CreateTestTarget();

			// Act

			await target.DeleteDisc(ReferenceData.NormalDiscId, CancellationToken.None);

			// Assert

			var discFromRepository = await GetDisc(ReferenceData.NormalDiscId);
			discFromRepository.AdviseGroup.Should().BeNull();

			var referenceData = GetReferenceData();
			var expectedAdviseGroups = new[]
			{
				referenceData.DiscAdviseGroup,
				referenceData.FolderAdviseGroup,
			};

			var adviseGroups = await GetAllAdviseGroups();
			adviseGroups.Should().BeEquivalentTo(expectedAdviseGroups, x => x.WithStrictOrdering());

			await CheckLibraryConsistency();
		}
	}
}
