using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using MusicLibrary.Core.Models;
using MusicLibrary.Services.Interfaces;
using MusicLibrary.Services.Interfaces.Dal;

namespace MusicLibrary.Services.UnitTests
{
	[TestClass]
	public class DiscsServiceTests
	{
		[TestMethod]
		public async Task UpdateDisc_TreeTitleWasNotChanged_DoesNotUpdateDiscTreeTitleInStorageRepository()
		{
			// Arrange

			static DiscModel CreateDiscData()
			{
				return new()
				{
					Id = new ItemId("Disc Id"),
					Folder = new ShallowFolderModel
					{
						Id = new ItemId("Folder Id"),
						Name = "Test Folder",
					},
					Title = "Old Disc Title",
					TreeTitle = "2021 - Some Disc (CD 1)",
				};
			}

			var existingDisc = CreateDiscData();
			var updatedDisc = CreateDiscData();
			updatedDisc.Title = "New Disc Title";

			var mocker = new AutoMocker();
			mocker.GetMock<IDiscsRepository>()
				.Setup(x => x.GetDisc(new ItemId("Disc Id"), CancellationToken.None))
				.ReturnsAsync(existingDisc);

			var target = mocker.CreateInstance<DiscsService>();

			// Act

			await target.UpdateDisc(updatedDisc, CancellationToken.None);

			// Assert

			var storageRepositoryMock = mocker.GetMock<IStorageRepository>();
			storageRepositoryMock.Verify(x => x.UpdateDiscTreeTitle(It.IsAny<DiscModel>(), It.IsAny<DiscModel>(), It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task UpdateDisc_AlbumTitleWasChanged_UpdatesEachActiveSong()
		{
			// Arrange

			static DiscModel CreateDiscData()
			{
				return new()
				{
					Id = new ItemId("Disc Id"),
					Folder = new ShallowFolderModel
					{
						Id = new ItemId("Folder Id"),
						Name = "Test Folder",
					},
					Year = 2021,
					Title = "Some Disc (CD 1)",
					TreeTitle = "2021 - Some Disc (CD 1)",
					AlbumTitle = "Some Disc",
					AllSongs = new SongModel[]
					{
						new() { Id = new ItemId("1") },
						new() { Id = new ItemId("2") },
						new()
						{
							Id = new ItemId("3"),
							DeleteDate = new DateTime(2021, 07, 03),
						},
					},
				};
			}

			var existingDisc = CreateDiscData();
			var updatedDisc = CreateDiscData();
			updatedDisc.AlbumTitle = "New Album Title";

			var mocker = new AutoMocker();
			mocker.GetMock<IDiscsRepository>()
				.Setup(x => x.GetDisc(new ItemId("Disc Id"), CancellationToken.None))
				.ReturnsAsync(existingDisc);

			var target = mocker.CreateInstance<DiscsService>();

			// Act

			await target.UpdateDisc(updatedDisc, CancellationToken.None);

			// Assert

			var activeSongs = updatedDisc.ActiveSongs.ToList();

			var songsServiceMock = mocker.GetMock<ISongsService>();
			songsServiceMock.Verify(x => x.UpdateSong(It.IsAny<SongModel>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
			songsServiceMock.Verify(x => x.UpdateSong(activeSongs[0], It.IsAny<CancellationToken>()), Times.Once);
			songsServiceMock.Verify(x => x.UpdateSong(activeSongs[1], It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task UpdateDisc_AlbumYearWasChanged_UpdatesEachActiveSong()
		{
			// Arrange

			static DiscModel CreateDiscData()
			{
				return new()
				{
					Id = new ItemId("Disc Id"),
					Folder = new ShallowFolderModel
					{
						Id = new ItemId("Folder Id"),
						Name = "Test Folder",
					},
					Title = "Some Disc (CD 1)",
					TreeTitle = "2021 - Some Disc (CD 1)",
					AlbumTitle = "Some Disc",
					AllSongs = new SongModel[]
					{
						new() { Id = new ItemId("1") },
						new() { Id = new ItemId("2") },
						new()
						{
							Id = new ItemId("3"),
							DeleteDate = new DateTime(2021, 07, 03),
						},
					},
				};
			}

			var existingDisc = CreateDiscData();
			var updatedDisc = CreateDiscData();
			updatedDisc.Year = 1988;

			var mocker = new AutoMocker();
			mocker.GetMock<IDiscsRepository>()
				.Setup(x => x.GetDisc(new ItemId("Disc Id"), CancellationToken.None))
				.ReturnsAsync(existingDisc);

			var target = mocker.CreateInstance<DiscsService>();

			// Act

			await target.UpdateDisc(updatedDisc, CancellationToken.None);

			// Assert

			var activeSongs = updatedDisc.ActiveSongs.ToList();

			var songsServiceMock = mocker.GetMock<ISongsService>();
			songsServiceMock.Verify(x => x.UpdateSong(It.IsAny<SongModel>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
			songsServiceMock.Verify(x => x.UpdateSong(activeSongs[0], It.IsAny<CancellationToken>()), Times.Once);
			songsServiceMock.Verify(x => x.UpdateSong(activeSongs[1], It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task UpdateDisc_IfTagsRelatedDataWasNotChanged_DoesNotUpdateSongs()
		{
			// Arrange

			static DiscModel CreateDiscData()
			{
				return new()
				{
					Id = new ItemId("Disc Id"),
					Folder = new ShallowFolderModel
					{
						Id = new ItemId("Folder Id"),
						Name = "Test Folder",
					},
					Title = "Some Disc (CD 1)",
					TreeTitle = "2021 - Some Disc (CD 1)",
					AlbumTitle = "Some Disc",
					AllSongs = new SongModel[]
					{
						new() { Id = new ItemId("1") },
					},
				};
			}

			var existingDisc = CreateDiscData();
			var updatedDisc = CreateDiscData();
			updatedDisc.Title = "New Title";
			updatedDisc.Title = "New Tree Title";

			var mocker = new AutoMocker();
			mocker.GetMock<IDiscsRepository>()
				.Setup(x => x.GetDisc(new ItemId("Disc Id"), CancellationToken.None))
				.ReturnsAsync(existingDisc);

			var target = mocker.CreateInstance<DiscsService>();

			// Act

			await target.UpdateDisc(updatedDisc, CancellationToken.None);

			// Assert

			var songsServiceMock = mocker.GetMock<ISongsService>();
			songsServiceMock.Verify(x => x.UpdateSong(It.IsAny<SongModel>(), It.IsAny<CancellationToken>()), Times.Never);
		}
	}
}
