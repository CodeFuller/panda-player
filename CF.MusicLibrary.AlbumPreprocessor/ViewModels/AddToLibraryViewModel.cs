using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using CF.Library.Core.Facades;
using CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary;
using CF.MusicLibrary.AlbumPreprocessor.MusicStorage;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels
{
	public class AddToLibraryViewModel : ViewModelBase
	{
		private const string coverImageFileName = "cover.jpg";

		private readonly IMusicLibrary musicLibrary;
		private readonly IWorkshopMusicStorage workshopStorage;
		private readonly IStorageUrlBuilder storageUrlBuilder;
		private readonly IWindowService windowService;
		private readonly ISongTagger songTagger;
		private readonly IFileSystemFacade fileSystemFacade;

		private AddedContent addedContent;

		public ObservableCollection<AddedAlbum> AddedAlbums => addedContent.Albums;

		public bool RequiredDataIsFilled => addedContent.RequiredDataIsFilled;

		public AddToLibraryViewModel(IMusicLibrary musicLibrary, IWorkshopMusicStorage workshopStorage, IStorageUrlBuilder storageUrlBuilder,
			ISongTagger songTagger, IWindowService windowService, IFileSystemFacade fileSystemFacade)
		{
			if (musicLibrary == null)
			{
				throw new ArgumentNullException(nameof(musicLibrary));
			}
			if (workshopStorage == null)
			{
				throw new ArgumentNullException(nameof(workshopStorage));
			}
			if (storageUrlBuilder == null)
			{
				throw new ArgumentNullException(nameof(storageUrlBuilder));
			}
			if (windowService == null)
			{
				throw new ArgumentNullException(nameof(windowService));
			}
			if (songTagger == null)
			{
				throw new ArgumentNullException(nameof(songTagger));
			}
			if (fileSystemFacade == null)
			{
				throw new ArgumentNullException(nameof(fileSystemFacade));
			}

			this.musicLibrary = musicLibrary;
			this.workshopStorage = workshopStorage;
			this.storageUrlBuilder = storageUrlBuilder;
			this.windowService = windowService;
			this.songTagger = songTagger;
			this.fileSystemFacade = fileSystemFacade;
		}

		public async Task AddAlbumsToLibrary(IEnumerable<AlbumTreeViewItem> albums)
		{
			if (albums == null)
			{
				throw new ArgumentNullException(nameof(albums));
			}

			addedContent = new AddedContent(musicLibrary, workshopStorage, storageUrlBuilder);
			addedContent.PropertyChanged += Property_Changed;

			await addedContent.SetAlbums(albums);
			if (windowService.ShowAddToLibraryWindow(this))
			{
				await SetTags();
				windowService.ShowMessageBox($"Successfully tagged {addedContent.SongsNumber} songs", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

				await StoreAlbumsInLibrary();
				windowService.ShowMessageBox($"Successfully added songs to library. Don't forget to reindex in MediaMonkey", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		private async Task SetTags()
		{
			foreach (AddedAlbum album in addedContent.Albums)
			{
				foreach (SongInfo song in album.Songs)
				{
					SongTagData songTagData = new SongTagData
					{
						Artist = album.GetSongArtist(song),
						Album = album.Title,
						Year = album.Year,
						Genre = album.Genre.Name,

						Track = song.Track,
						Title = song.Title,
					};

					await songTagger.SetTagData(song.SourcePath, songTagData);
				}
			}
		}

		private async Task StoreAlbumsInLibrary()
		{
			foreach (AddedAlbum album in addedContent.Albums)
			{
				foreach (SongInfo song in album.Songs)
				{
					//	Currently we don't add song to IMusicCatalog, only to IMusicStorage.
					//	If this is changed we should fill all other Song fields like Artist, Title, FileSize, Bitrate, ...
					Song addedSong = new Song
					{
						Uri = storageUrlBuilder.BuildSongStorageUrl(album.DestinationUri, song.SourceFileName)
					};

					await musicLibrary.AddSong(addedSong, song.SourcePath);
				}

				//	Copying cover image
				var coverImagePath = Path.Combine(album.SourcePath, coverImageFileName);
				if (fileSystemFacade.FileExists(coverImagePath))
				{
					await musicLibrary.SetAlbumCoverImage(album.DestinationUri, coverImagePath);
				}
			}
		}

		private void Property_Changed(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(AddedAlbum.RequiredDataIsFilled))
			{
				RaisePropertyChanged(nameof(RequiredDataIsFilled));
			}
		}

	}
}
