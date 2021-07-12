using System;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.IntegrationTests.Data
{
	public partial class ReferenceData
	{
		public static ItemId RootFolderId => new("1");

		public static ItemId SubFolderId => new("2");

		public static ItemId ArtistFolderId => new("3");

		public static ItemId EmptyFolderId => new("4");

		public static ItemId DeletedFolderId => new("5");

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
			ParentFolderId = RootFolderId,
			Name = "Belarusian",
		};

		public ShallowFolderModel ArtistFolder { get; } = new()
		{
			Id = ArtistFolderId,
			ParentFolderId = SubFolderId,
			Name = "Neuro Dubel",
		};

		public ShallowFolderModel EmptyFolder { get; } = new()
		{
			Id = EmptyFolderId,
			ParentFolderId = ArtistFolderId,
			Name = "Empty Folder",
		};

		public ShallowFolderModel DeletedFolder { get; } = new()
		{
			Id = DeletedFolderId,
			ParentFolderId = ArtistFolderId,
			Name = "Deleted Folder",
			DeleteDate = new DateTimeOffset(2021, 06, 30, 18, 08, 10, TimeSpan.FromHours(3)),
		};
	}
}
