using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CF.Library.Core.Exceptions;
using CF.Library.Core.Facades;
using CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary;
using CF.MusicLibrary.AlbumPreprocessor.MusicStorage;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels.Interfaces;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels
{
	public class EditAlbumsDetailsViewModel : ViewModelBase, IEditAlbumsDetailsViewModel
	{
		private const string CoverImageFileName = "cover.jpg";

		private readonly IMusicLibrary musicLibrary;
		private readonly IWorkshopMusicStorage workshopStorage;
		private readonly IStorageUrlBuilder storageUrlBuilder;
		private readonly IFileSystemFacade fileSystemFacade;

		public ObservableCollection<AddedAlbum> Albums { get; private set; }

		public IEnumerable<AddedAlbumCoverImage> AlbumCoverImages
		{
			get
			{
				foreach (AddedAlbum album in Albums)
				{
					var coverImagePath = Path.Combine(album.SourcePath, CoverImageFileName);
					if (fileSystemFacade.FileExists(coverImagePath))
					{
						yield return new AddedAlbumCoverImage(album.DestinationUri, coverImagePath);
					}
				}
			}
		}

		public bool RequiredDataIsFilled => Albums.All(a => a.RequiredDataIsFilled);

		public IEnumerable<TaggedSongData> Songs
		{
			get
			{
				foreach (AddedAlbum album in Albums)
				{
					foreach (SongInfo song in album.Songs)
					{
						yield return new TaggedSongData
						{
							SourceFileName = song.SourcePath,
							StorageUri = storageUrlBuilder.BuildSongStorageUrl(album.DestinationUri, song.SourceFileName),

							Artist = album.GetSongArtist(song),
							Album = album.Title,
							Year = album.Year,
							Genre = album.Genre.Name,

							Track = song.Track,
							Title = song.Title,
						};
					}
				}
			}
		}

		public EditAlbumsDetailsViewModel(IMusicLibrary musicLibrary, IWorkshopMusicStorage workshopStorage,
			IStorageUrlBuilder storageUrlBuilder, IFileSystemFacade fileSystemFacade)
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
			if (fileSystemFacade == null)
			{
				throw new ArgumentNullException(nameof(fileSystemFacade));
			}

			this.musicLibrary = musicLibrary;
			this.workshopStorage = workshopStorage;
			this.storageUrlBuilder = storageUrlBuilder;
			this.fileSystemFacade = fileSystemFacade;
		}

		public async Task SetAlbums(IEnumerable<AlbumTreeViewItem> albums)
		{
			if (albums == null)
			{
				throw new ArgumentNullException(nameof(albums));
			}

			Albums = new ObservableCollection<AddedAlbum>();

			List<Genre> libraryGenres = (await musicLibrary.GetGenresAsync()).ToList();

			foreach (var album in albums)
			{
				AlbumInfo albumInfo = workshopStorage.GetAlbumInfo(album.AlbumDirectory, album.SongFileNames);

				if (albumInfo.Songs.Count != album.Songs.Count())
				{
					throw new InvalidOperationException($"Strange songs number in '{album.AlbumDirectory}'");
				}

				AddedAlbum addedAlbum;
				switch (albumInfo.AlbumType)
				{
					case AlbumType.ArtistAlbum:
						{
							var artistStorageUri = musicLibrary.GetArtistStorageUri(albumInfo.Artist);
							IEnumerable<Uri> artistStorageUris = artistStorageUri != null
								? Enumerable.Repeat(artistStorageUri, 1)
								: musicLibrary.GetAvailableArtistStorageUris(albumInfo.Artist);

							addedAlbum = new AddedArtistAlbum(album.AlbumDirectory, albumInfo,
								artistStorageUris.Select(u => storageUrlBuilder.BuildAlbumStorageUrl(u, albumInfo.NameInStorage)),
								artistStorageUri != null ? storageUrlBuilder.BuildAlbumStorageUrl(artistStorageUri, albumInfo.NameInStorage) : null,
								libraryGenres,
								PredictArtistGenre(albumInfo.Artist));
						}
						break;

					case AlbumType.CompilationAlbumWithArtistInfo:
						addedAlbum = new AddedCompilationAlbumWithArtistInfo(album.AlbumDirectory, albumInfo, storageUrlBuilder.MapWorkshopStoragePath(albumInfo.PathWithinStorage), libraryGenres);
						break;

					case AlbumType.CompilationAlbumWithoutArtistInfo:
						addedAlbum = new AddedCompilationAlbumWithoutArtistInfo(album.AlbumDirectory, albumInfo, storageUrlBuilder.MapWorkshopStoragePath(albumInfo.PathWithinStorage), libraryGenres);
						break;

					default:
						throw new UnexpectedEnumValueException(albumInfo.AlbumType);
				}

				addedAlbum.PropertyChanged += Property_Changed;

				Albums.Add(addedAlbum);
			}
		}

		private void Property_Changed(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(AddedAlbum.RequiredDataIsFilled))
			{
				RaisePropertyChanged(nameof(RequiredDataIsFilled));
			}

			AddedArtistAlbum changedAlbum = sender as AddedArtistAlbum;
			if (changedAlbum != null)
			{
				if (e.PropertyName == nameof(AddedAlbum.Genre))
				{
					FillSameArtistAlbumsData(changedAlbum, a => a.Genre == null, a => a.Genre = changedAlbum.Genre);
				}
				else if (e.PropertyName == nameof(AddedAlbum.DestinationUri))
				{
					FillSameArtistAlbumsData(changedAlbum, a => a.DestinationUri == null, a => a.DestinationUri = storageUrlBuilder.ReplaceAlbumName(changedAlbum.DestinationUri, a.NameInStorage));
				}
			}
		}

		private void FillSameArtistAlbumsData(AddedArtistAlbum changedAlbum, Func<AddedArtistAlbum, bool> valueIsEmpty, Action<AddedArtistAlbum> setValue)
		{
			foreach (var album in Albums.Where(a => a != changedAlbum && a.Artist == changedAlbum.Artist))
			{
				AddedArtistAlbum artistAlbum = album as AddedArtistAlbum;
				if (artistAlbum != null && valueIsEmpty(artistAlbum))
				{
					setValue(artistAlbum);
				}
			}
		}

		private Genre PredictArtistGenre(string artist)
		{
			//	Selecting genre of the latest album
			var discs = musicLibrary.ArtistLibrary.GetArtistDiscs(artist);
			return discs?.OrderByDescending(d => d.Year).
					  Where(d => d.Genre != null).
					  Select(d => d.Genre).FirstOrDefault();
		}
	}
}
