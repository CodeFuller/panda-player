using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CF.MusicLibrary.Core;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class EditSongPropertiesViewModel : ViewModelBase, IEditSongPropertiesViewModel
	{
		private readonly DiscLibrary library;
		private readonly ILibraryStructurer libraryStructurer;
		private readonly ILibraryContentUpdater libraryContentUpdater;

		private List<Song> editedSongs;

		public bool SingleSongMode => editedSongs.Count == 1;

		private Song SingleSong
		{
			get
			{
				if (!SingleSongMode)
				{
					throw new InvalidOperationException("Not in a single song mode");
				}

				return editedSongs.Single();
			}
		}

		private string fileName;
		public string FileName
		{
			get { return fileName; }
			set
			{
				if (!SingleSongMode)
				{
					throw new InvalidOperationException("File name could not be edited in multi-song mode");
				}
				if (String.IsNullOrWhiteSpace(value))
				{
					throw new InvalidOperationException("Value of song file name could not be empty");
				}

				Set(ref fileName, value);
			}
		}

		private Uri UpdatedSongUri
		{
			get
			{
				if (!SingleSongMode)
				{
					throw new InvalidOperationException("Uri could not be updated in multi-song mode");
				}

				return String.Equals(FileName, GetSongFileName(SingleSong), StringComparison.OrdinalIgnoreCase) ?
					null : libraryStructurer.BuildSongUri(SingleSong.Disc.Uri, FileName);
			}
		}

		private string title;
		public string Title
		{
			get { return title; }
			set
			{
				if (!SingleSongMode)
				{
					throw new InvalidOperationException("Title could not be edited in multi-song mode");
				}
				if (String.IsNullOrWhiteSpace(value))
				{
					throw new InvalidOperationException("Value of song title could not be empty");
				}

				Set(ref title, value);
			}
		}

		public EditedSongProperty<Artist> Artist { get; set; }

		public EditedSongProperty<Genre> Genre { get; set; }

		public EditedSongProperty<short?> Year { get; set; }

		public EditedSongProperty<short?> TrackNumber { get; set; }

		public IEnumerable<EditedSongProperty<Artist>> AvailableArtists { get; private set; }

		public IEnumerable<EditedSongProperty<Genre>> AvailableGenres { get; private set; }

		public EditSongPropertiesViewModel(DiscLibrary library, ILibraryStructurer libraryStructurer, ILibraryContentUpdater libraryContentUpdater)
		{
			if (library == null)
			{
				throw new ArgumentNullException(nameof(library));
			}
			if (libraryStructurer == null)
			{
				throw new ArgumentNullException(nameof(libraryStructurer));
			}
			if (libraryContentUpdater == null)
			{
				throw new ArgumentNullException(nameof(libraryContentUpdater));
			}

			this.library = library;
			this.libraryStructurer = libraryStructurer;
			this.libraryContentUpdater = libraryContentUpdater;
		}

		public void Load(IEnumerable<Song> songs)
		{
			editedSongs = songs.ToList();
			if (editedSongs.Count == 0)
			{
				throw new InvalidOperationException("Songs list is empty");
			}

			AvailableArtists = FillAvailableValues(library.AllArtists, a => a.Name);
			AvailableGenres = FillAvailableValues(library.Genres, g => g.Name);

			fileName = SingleSongMode ? GetSongFileName(SingleSong) : null;
			title = SingleSongMode ? SingleSong.Title : null;
			Artist = BuildProperty(editedSongs, s => s.Artist);
			Genre = BuildProperty(editedSongs, s => s.Genre);
			Year = BuildProperty(editedSongs, s => s.Year);
			TrackNumber = BuildProperty(editedSongs, s => s.TrackNumber);
		}

		public async Task Save()
		{
			//	Should we rename a song?
			if (editedSongs.Count == 1)
			{
				Uri newSongUri = UpdatedSongUri;
				if (newSongUri != null)
				{
					await libraryContentUpdater.ChangeSongUri(SingleSong, newSongUri);
				}
			}

			await libraryContentUpdater.UpdateSongs(GetUpdatedSongs(), UpdatedSongProperties.ForceTagUpdate);
		}

		private static IEnumerable<EditedSongProperty<T>> FillAvailableValues<T>(IEnumerable<T> values, Func<T, object> sortedProperty) where T : class
		{
			var availableValues = new List<EditedSongProperty<T>>();
			availableValues.Add(new EditedSongProperty<T>());
			availableValues.Add(new EditedSongProperty<T>(null));
			availableValues.AddRange(values.OrderBy(sortedProperty).Select(v => new EditedSongProperty<T>(v)));
			return availableValues;
		}

		private IEnumerable<Song> GetUpdatedSongs()
		{
			foreach (var song in editedSongs)
			{
				if (SingleSongMode)
				{
					song.Title = Title;
				}

				if (Artist.HasValue)
				{
					song.Artist = Artist.Value;
				}
				if (Genre.HasValue)
				{
					song.Genre = Genre.Value;
				}
				if (Year.HasValue)
				{
					song.Year = Year.Value;
				}
				if (TrackNumber.HasValue)
				{
					song.TrackNumber = TrackNumber.Value;
				}

				yield return song;
			}
		}

		private static EditedSongProperty<T> BuildProperty<T>(IEnumerable<Song> songs, Func<Song, T> propertyAccessor)
		{
			var propertyValues = songs.Select(propertyAccessor).Distinct().ToList();
			return propertyValues.Count == 1 ? new EditedSongProperty<T>(propertyValues.Single()) : new EditedSongProperty<T>();
		}

		private string GetSongFileName(Song song)
		{
			return libraryStructurer.GetFileNameFromUri(song.Uri);
		}
	}
}
