using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces.Dal;

namespace PandaPlayer.Services.UnitTests
{
	[TestClass]
	public class DiscsServiceTests
	{
		[TestMethod]
		public async Task UpdateDisc_TreeTitleWasNotChanged_DoesNotUpdateDiscTreeTitleInStorageRepository()
		{
			// Arrange

			var disc = new DiscModel { Id = new ItemId("Disc Id"), Title = "Old Disc Title", TreeTitle = "2021 - Some Disc (CD 1)" };
			disc.AddSong(new SongModel());

			var folder = new FolderModel { Id = new ItemId("Folder Id"), Name = "Test Folder" };
			folder.AddDisc(disc);

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscsService>();

			// Act

			static void UpdateDisc(DiscModel updatedDisc)
			{
				updatedDisc.Title = "New Disc Title";
			}

			await target.UpdateDisc(disc, UpdateDisc, CancellationToken.None);

			// Assert

			var storageRepositoryMock = mocker.GetMock<IStorageRepository>();
			storageRepositoryMock.Verify(x => x.UpdateDiscTreeTitle(It.IsAny<DiscModel>(), It.IsAny<DiscModel>(), It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task UpdateDisc_AlbumTitleWasChanged_UpdatesEachActiveSong()
		{
			// Arrange

			var disc = new DiscModel
			{
				Id = new ItemId("Disc Id"),
				Year = 2021,
				Title = "Some Disc (CD 1)",
				TreeTitle = "2021 - Some Disc (CD 1)",
				AlbumTitle = "Some Disc",
			};

			disc.AddSong(new() { Id = new ItemId("1") });
			disc.AddSong(new() { Id = new ItemId("2") });
			disc.AddSong(new() { Id = new ItemId("3"), DeleteDate = new DateTime(2021, 07, 03) });

			var folder = new FolderModel { Id = new ItemId("Folder Id"), Name = "Test Folder" };
			folder.AddDisc(disc);

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscsService>();

			// Act

			static void UpdateDisc(DiscModel updatedDisc)
			{
				updatedDisc.AlbumTitle = "New Album Title";
			}

			await target.UpdateDisc(disc, UpdateDisc, CancellationToken.None);

			// Assert

			var activeSongs = disc.ActiveSongs.ToList();

			var songsRepositoryMock = mocker.GetMock<ISongsRepository>();
			songsRepositoryMock.Verify(x => x.UpdateSong(It.IsAny<SongModel>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
			songsRepositoryMock.Verify(x => x.UpdateSong(activeSongs[0], It.IsAny<CancellationToken>()), Times.Once);
			songsRepositoryMock.Verify(x => x.UpdateSong(activeSongs[1], It.IsAny<CancellationToken>()), Times.Once);

			var storageRepositoryMock = mocker.GetMock<IStorageRepository>();
			storageRepositoryMock.Verify(x => x.UpdateSong(It.IsAny<SongModel>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
			storageRepositoryMock.Verify(x => x.UpdateSong(activeSongs[0], It.IsAny<CancellationToken>()), Times.Once);
			storageRepositoryMock.Verify(x => x.UpdateSong(activeSongs[1], It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task UpdateDisc_AlbumYearWasChanged_UpdatesEachActiveSong()
		{
			// Arrange

			var disc = new DiscModel
			{
				Id = new ItemId("Disc Id"),
				Title = "Some Disc (CD 1)",
				TreeTitle = "2021 - Some Disc (CD 1)",
				AlbumTitle = "Some Disc",
			};

			disc.AddSong(new() { Id = new ItemId("1") });
			disc.AddSong(new() { Id = new ItemId("2") });
			disc.AddSong(new() { Id = new ItemId("3"), DeleteDate = new DateTime(2021, 07, 03) });

			var folder = new FolderModel { Id = new ItemId("Folder Id"), Name = "Test Folder" };
			folder.AddDisc(disc);

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscsService>();

			// Act

			static void UpdateDisc(DiscModel updatedDisc)
			{
				updatedDisc.Year = 1988;
			}

			await target.UpdateDisc(disc, UpdateDisc, CancellationToken.None);

			// Assert

			var activeSongs = disc.ActiveSongs.ToList();

			var songsRepositoryMock = mocker.GetMock<ISongsRepository>();
			songsRepositoryMock.Verify(x => x.UpdateSong(It.IsAny<SongModel>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
			songsRepositoryMock.Verify(x => x.UpdateSong(activeSongs[0], It.IsAny<CancellationToken>()), Times.Once);
			songsRepositoryMock.Verify(x => x.UpdateSong(activeSongs[1], It.IsAny<CancellationToken>()), Times.Once);

			var storageRepositoryMock = mocker.GetMock<IStorageRepository>();
			storageRepositoryMock.Verify(x => x.UpdateSong(It.IsAny<SongModel>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
			storageRepositoryMock.Verify(x => x.UpdateSong(activeSongs[0], It.IsAny<CancellationToken>()), Times.Once);
			storageRepositoryMock.Verify(x => x.UpdateSong(activeSongs[1], It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task UpdateDisc_IfTagsRelatedDataWasNotChanged_DoesNotUpdateSongs()
		{
			// Arrange

			var disc = new DiscModel
			{
				Id = new ItemId("Disc Id"),
				Title = "Some Disc (CD 1)",
				TreeTitle = "2021 - Some Disc (CD 1)",
				AlbumTitle = "Some Disc",
			};

			disc.AddSong(new() { Id = new ItemId("1") });

			var folder = new FolderModel { Id = new ItemId("Folder Id"), Name = "Test Folder" };
			folder.AddDisc(disc);

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscsService>();

			// Act

			static void UpdateDisc(DiscModel updatedDisc)
			{
				updatedDisc.Title = "New Title";
				updatedDisc.Title = "New Tree Title";
			}

			await target.UpdateDisc(disc, UpdateDisc, CancellationToken.None);

			// Assert

			var songsRepositoryMock = mocker.GetMock<ISongsRepository>();
			songsRepositoryMock.Verify(x => x.UpdateSong(It.IsAny<SongModel>(), It.IsAny<CancellationToken>()), Times.Never);

			var storageRepositoryMock = mocker.GetMock<IStorageRepository>();
			storageRepositoryMock.Verify(x => x.UpdateSong(It.IsAny<SongModel>(), It.IsAny<CancellationToken>()), Times.Never);
		}
	}
}
