using System;
using System.Collections.Generic;
using System.Linq;
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

		public FolderModel RootFolder { get; private set; }

		public FolderModel SubFolder { get; private set; }

		public FolderModel ArtistFolder { get; private set; }

		public FolderModel EmptyFolder { get; private set; }

		public FolderModel DeletedFolder { get; private set; }

		private IEnumerable<FolderModel> AllFolders
		{
			get
			{
				yield return RootFolder;
				yield return SubFolder;
				yield return ArtistFolder;
				yield return EmptyFolder;
				yield return DeletedFolder;
			}
		}

		private void FillFolders()
		{
			RootFolder = new()
			{
				Id = RootFolderId,
				Name = "<ROOT>",
			};

			SubFolder = new()
			{
				Id = SubFolderId,
				Name = "Belarusian",
			};

			ArtistFolder = new()
			{
				Id = ArtistFolderId,
				Name = "Neuro Dubel",
				AdviseGroup = FolderAdviseGroup,
			};

			EmptyFolder = new()
			{
				Id = EmptyFolderId,
				Name = "Empty Folder",
			};

			DeletedFolder = new()
			{
				Id = DeletedFolderId,
				Name = "Deleted Folder",
				DeleteDate = new DateTimeOffset(2021, 06, 30, 18, 08, 10, TimeSpan.FromHours(3)),
			};

			RootFolder.AddSubfolder(SubFolder);
			SubFolder.AddSubfolder(ArtistFolder);
			ArtistFolder.AddSubfolder(EmptyFolder);
			ArtistFolder.AddSubfolder(DeletedFolder);
		}

		public FolderModel GetFolder(ItemId folderId)
		{
			return AllFolders.Single(x => x.Id == folderId);
		}
	}
}
