using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MusicLibrary.Core;
using MusicLibrary.Core.Interfaces;
using MusicLibrary.Core.Objects;
using MusicLibrary.PandaPlayer.ContentUpdate;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace MusicLibrary.PandaPlayer.ViewModels
{
	public class EditDiscPropertiesViewModel : ViewModelBase, IEditDiscPropertiesViewModel
	{
		private readonly ILibraryStructurer libraryStructurer;
		private readonly ILibraryContentUpdater libraryContentUpdater;

		private Disc Disc { get; set; }

		private string folderName;

		public string FolderName
		{
			get => folderName;
			set
			{
				if (String.IsNullOrWhiteSpace(value))
				{
					throw new InvalidOperationException("Value of Disc folder name could not be empty");
				}

				Set(ref folderName, value);
			}
		}

		private string discTitle;

		public string DiscTitle
		{
			get => discTitle;
			set
			{
				if (String.IsNullOrWhiteSpace(value))
				{
					throw new InvalidOperationException("Value of Disc title could not be empty");
				}

				Set(ref discTitle, value);
			}
		}

		private string albumTitle;

		public string AlbumTitle
		{
			get => albumTitle;
			set
			{
				if (value?.Length == 0)
				{
					value = null;
				}

				Set(ref albumTitle, value);
			}
		}

		public EditDiscPropertiesViewModel(ILibraryStructurer libraryStructurer, ILibraryContentUpdater libraryContentUpdater)
		{
			this.libraryStructurer = libraryStructurer ?? throw new ArgumentNullException(nameof(libraryStructurer));
			this.libraryContentUpdater = libraryContentUpdater ?? throw new ArgumentNullException(nameof(libraryContentUpdater));
		}

		public void Load(Disc disc)
		{
			Disc = disc;
			FolderName = libraryStructurer.GetDiscFolderName(disc.Uri);
			DiscTitle = disc.Title;
			AlbumTitle = disc.AlbumTitle;
		}

		public async Task Save()
		{
			var updatedProperties = UpdatedSongProperties.None;
			if (!String.Equals(Disc.AlbumTitle, AlbumTitle, StringComparison.Ordinal))
			{
				updatedProperties |= UpdatedSongProperties.Album;
			}

			Disc.Title = DiscTitle;
			Disc.AlbumTitle = AlbumTitle;

			var originalDiscFolderName = libraryStructurer.GetDiscFolderName(Disc.Uri);
			if (!String.Equals(originalDiscFolderName, FolderName, StringComparison.Ordinal))
			{
				await libraryContentUpdater.ChangeDiscUri(Disc, libraryStructurer.ReplaceDiscPartInUri(Disc.Uri, FolderName));
			}

			await libraryContentUpdater.UpdateDisc(Disc, updatedProperties);
		}
	}
}
