using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CF.Library.Core.Facades;
using CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels
{
	public class AddToLibraryViewModel : ViewModelBase
	{
		private readonly EditAlbumsDetailsViewModel editAlbumsDetailsViewModel;
		private readonly EditSongsDetailsViewModel editSongsDetailsViewModel;
		private readonly ISongTagger songTagger;
		private readonly IWindowService windowService;
		private readonly IMusicLibrary musicLibrary;
		private readonly IFileSystemFacade fileSystemFacade;

		public AddToLibraryViewModel(EditAlbumsDetailsViewModel editAlbumsDetailsViewModel, EditSongsDetailsViewModel editSongsDetailsViewModel,
			ISongTagger songTagger, IWindowService windowService, IMusicLibrary musicLibrary, IFileSystemFacade fileSystemFacade)
		{
			if (editAlbumsDetailsViewModel == null)
			{
				throw new ArgumentNullException(nameof(editAlbumsDetailsViewModel));
			}
			if (songTagger == null)
			{
				throw new ArgumentNullException(nameof(songTagger));
			}
			if (windowService == null)
			{
				throw new ArgumentNullException(nameof(windowService));
			}
			if (musicLibrary == null)
			{
				throw new ArgumentNullException(nameof(musicLibrary));
			}
			if (fileSystemFacade == null)
			{
				throw new ArgumentNullException(nameof(fileSystemFacade));
			}

			this.editAlbumsDetailsViewModel = editAlbumsDetailsViewModel;
			this.editSongsDetailsViewModel = editSongsDetailsViewModel;
			this.songTagger = songTagger;
			this.windowService = windowService;
			this.musicLibrary = musicLibrary;
			this.fileSystemFacade = fileSystemFacade;
		}

		public virtual async Task AddAlbumsToLibrary(IEnumerable<AlbumTreeViewItem> albums)
		{
			if (albums == null)
			{
				throw new ArgumentNullException(nameof(albums));
			}

			await musicLibrary.LoadAsync();

			await editAlbumsDetailsViewModel.SetAlbums(albums);
			if (!windowService.ShowEditAlbumsDetailsWindow(editAlbumsDetailsViewModel))
			{
				return;
			}

			editSongsDetailsViewModel.SetSongs(editAlbumsDetailsViewModel.Songs);
			if (!windowService.ShowEditSongsDetailsWindow(editSongsDetailsViewModel))
			{
				return;
			}

			List<TaggedSongData> songsTagData = editSongsDetailsViewModel.Songs.Select(s => s.TagData).ToList();

			await SetTags(songsTagData);
			windowService.ShowMessageBox($"Successfully tagged {editSongsDetailsViewModel.Songs.Count} songs", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

			await StoreAlbumsInLibrary(songsTagData, editAlbumsDetailsViewModel.AlbumCoverImages);
			windowService.ShowMessageBox("Successfully added songs to the library. Don't forget to reindex in MediaMonkey", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private async Task SetTags(IEnumerable<TaggedSongData> songs)
		{
			foreach (TaggedSongData song in songs)
			{
				fileSystemFacade.ClearReadOnlyAttribute(song.SourceFileName);
				await songTagger.SetTagData(song);
			}
		}

		private async Task StoreAlbumsInLibrary(IEnumerable<TaggedSongData> songs, IEnumerable<AddedAlbumCoverImage> albumCoverImages)
		{
			foreach (TaggedSongData song in songs)
			{
				//	Currently we don't add song to IMusicCatalog, only to IMusicStorage.
				//	If this is changed we should fill all other Song fields like Artist, Title, FileSize, Bitrate, ...
				Song addedSong = new Song
				{
					Uri = song.StorageUri
				};

				await musicLibrary.AddSong(addedSong, song.SourceFileName);
			}

			foreach (AddedAlbumCoverImage coverImage in albumCoverImages)
			{
				await musicLibrary.SetAlbumCoverImage(coverImage.AlbumStorageUri, coverImage.CoverImageFileName);
			}
		}
	}
}
