using System;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.IntegrationTests.Data
{
	public partial class TestData
	{
		public ShallowFolderModel RootFolder { get; } = new()
		{
			Id = new ItemId("1"),
			ParentFolderId = null,
			Name = "<ROOT>",
		};

		public ShallowFolderModel SubFolder { get; } = new()
		{
			Id = new ItemId("2"),
			ParentFolderId = new ItemId("1"),
			Name = "Foreign",
		};

		public ShallowFolderModel ArtistFolder { get; } = new()
		{
			Id = new ItemId("3"),
			ParentFolderId = new ItemId("2"),
			Name = "Guano Apes",
		};

		public ShallowFolderModel EmptyFolder { get; } = new()
		{
			Id = new ItemId("4"),
			ParentFolderId = new ItemId("3"),
			Name = "Empty Folder",
		};

		public ShallowFolderModel DeletedFolder { get; } = new()
		{
			Id = new ItemId("5"),
			ParentFolderId = new ItemId("3"),
			Name = "Deleted Folder",
			DeleteDate = new DateTimeOffset(2021, 06, 30, 18, 08, 10, TimeSpan.FromHours(3)),
		};
	}
}
