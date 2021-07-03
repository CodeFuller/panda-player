using System;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.IntegrationTests.Data
{
	public partial class ReferenceData
	{
		public static ItemId RootFolderId => new("1");

		public static ItemId SubFolderId => new("2");

		public static ItemId ArtistFolderId => new("3");

		public static ItemId EmptyFolderId => new("4");

		public static ItemId NextFolderId => new("6");

		public ShallowFolderModel RootFolder { get; } = new()
		{
			Id = RootFolderId,
			ParentFolderId = null,
			Name = "<ROOT>",
		};

		public ShallowFolderModel SubFolder { get; } = new()
		{
			Id = SubFolderId,
			ParentFolderId = new ItemId("1"),
			Name = "Foreign",
		};

		public ShallowFolderModel ArtistFolder { get; } = new()
		{
			Id = ArtistFolderId,
			ParentFolderId = new ItemId("2"),
			Name = "Guano Apes",
		};

		public ShallowFolderModel EmptyFolder { get; } = new()
		{
			Id = EmptyFolderId,
			ParentFolderId = ArtistFolderId,
			Name = "Empty Folder",
		};

		public ShallowFolderModel DeletedFolder { get; } = new()
		{
			Id = new ItemId("5"),
			ParentFolderId = ArtistFolderId,
			Name = "Deleted Folder",
			DeleteDate = new DateTimeOffset(2021, 06, 30, 18, 08, 10, TimeSpan.FromHours(3)),
		};
	}
}
