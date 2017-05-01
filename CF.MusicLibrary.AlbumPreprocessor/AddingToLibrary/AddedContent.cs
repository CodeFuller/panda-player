using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CF.Library.Core.Exceptions;
using CF.MusicLibrary.AlbumPreprocessor.MusicStorage;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary
{
	public class AddedContent : ViewModelBase
	{
		private readonly IMusicLibrary musicLibrary;
		private readonly IWorkshopMusicStorage workshopStorage;
		private readonly IStorageUrlBuilder storageUrlBuilder;

		public ObservableCollection<AddedAlbum> Albums { get; private set; }

		public int SongsNumber => Albums.Sum(a => a.Songs.Count);

		public bool RequiredDataIsFilled => Albums.All(a => a.RequiredDataIsFilled);

		public AddedContent(IMusicLibrary musicLibrary, IWorkshopMusicStorage workshopStorage, IStorageUrlBuilder storageUrlBuilder)
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

			this.musicLibrary = musicLibrary;
			this.workshopStorage = workshopStorage;
			this.storageUrlBuilder = storageUrlBuilder;
		}

		public async Task SetAlbums(IEnumerable<AlbumTreeViewItem> albums)
		{
			if (albums == null)
			{
				throw new ArgumentNullException(nameof(albums));
			}

			Albums = new ObservableCollection<AddedAlbum>();

			await musicLibrary.LoadAsync();
			List<Genre> libraryGenres = (await musicLibrary.GetGenresAsync()).ToList();

			foreach (var album in albums)
			{
				AlbumInfo albumInfo = workshopStorage.GetAlbumInfo(album.AlbumDirectory, album.Songs.Select(s => s.SongFileName));

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
