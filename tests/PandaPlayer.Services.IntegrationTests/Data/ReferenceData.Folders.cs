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

		public ShallowFolderModel RootFolder { get; private set; }

		public ShallowFolderModel SubFolder { get; private set; }

		public ShallowFolderModel ArtistFolder { get; private set; }

		public ShallowFolderModel EmptyFolder { get; private set; }

		public ShallowFolderModel DeletedFolder { get; private set; }

		private void FillFolders()
		{
			RootFolder = new()
			{
				Id = RootFolderId,
				ParentFolderId = null,
				Name = "<ROOT>",
			};

			SubFolder = new()
			{
				Id = SubFolderId,
				ParentFolderId = RootFolderId,
				Name = "Belarusian",
			};

			ArtistFolder = new()
			{
				Id = ArtistFolderId,
				ParentFolderId = SubFolderId,
				Name = "Neuro Dubel",
				AdviseGroup = FolderAdviseGroup,
			};

			EmptyFolder = new()
			{
				Id = EmptyFolderId,
				ParentFolderId = ArtistFolderId,
				Name = "Empty Folder",
			};

			DeletedFolder = new()
			{
				Id = DeletedFolderId,
				ParentFolderId = ArtistFolderId,
				Name = "Deleted Folder",
				DeleteDate = new DateTimeOffset(2021, 06, 30, 18, 08, 10, TimeSpan.FromHours(3)),
			};
		}
	}
}
