using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using MusicLibrary.Services.Comparers;
using MusicLibrary.Services.Interfaces;

namespace MusicLibrary.PandaPlayer.ViewModels
{
	public class EditSongPropertiesViewModel : ViewModelBase, IEditSongPropertiesViewModel
	{
		private readonly ISongsService songsService;
		private readonly IGenresService genresService;
		private readonly IArtistsService artistsService;

		private List<SongModel> editedSongs;

		public bool SingleSongMode => editedSongs.Count == 1;

		private SongModel SingleSong
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

		private string treeTitle;

		public string TreeTitle
		{
			get => treeTitle;
			set
			{
				if (!SingleSongMode)
				{
					throw new InvalidOperationException("Tree title could not be edited in multi-song mode");
				}

				if (String.IsNullOrWhiteSpace(value))
				{
					throw new InvalidOperationException("Value of song tree title could not be empty");
				}

				Set(ref treeTitle, value);
			}
		}

		private string title;

		public string Title
		{
			get => title;
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

		public EditedSongProperty<ArtistModel> Artist { get; set; }

		public EditedSongProperty<GenreModel> Genre { get; set; }

		public EditedSongProperty<short?> TrackNumber { get; set; }

		public IReadOnlyCollection<EditedSongProperty<ArtistModel>> AvailableArtists { get; private set; }

		public IReadOnlyCollection<EditedSongProperty<GenreModel>> AvailableGenres { get; private set; }

		public EditSongPropertiesViewModel(ISongsService songsService, IGenresService genresService, IArtistsService artistsService)
		{
			this.songsService = songsService ?? throw new ArgumentNullException(nameof(songsService));
			this.genresService = genresService ?? throw new ArgumentNullException(nameof(genresService));
			this.artistsService = artistsService ?? throw new ArgumentNullException(nameof(artistsService));
		}

		public async Task Load(IEnumerable<SongModel> songs, CancellationToken cancellationToken)
		{
			editedSongs = songs.ToList();
			if (editedSongs.Count == 0)
			{
				throw new InvalidOperationException("Songs list is empty");
			}

			await LoadArtists(cancellationToken);
			await LoadGenres(cancellationToken);

			treeTitle = SingleSongMode ? SingleSong.TreeTitle : null;
			title = SingleSongMode ? SingleSong.Title : null;

			TrackNumber = BuildProperty(editedSongs, s => s.TrackNumber, EqualityComparer<short?>.Default);
		}

		private async Task LoadArtists(CancellationToken cancellationToken)
		{
			var allArtists = await artistsService.GetAllArtists(cancellationToken);
			AvailableArtists = GetAvailableListItems(allArtists);
			Artist = SelectCurrentListItem(AvailableArtists, s => s.Artist, new ArtistEqualityComparer());
		}

		private async Task LoadGenres(CancellationToken cancellationToken)
		{
			var allGenres = await genresService.GetAllGenres(cancellationToken);
			AvailableGenres = GetAvailableListItems(allGenres);
			Genre = SelectCurrentListItem(AvailableGenres, s => s.Genre, new GenreEqualityComparer());
		}

		public async Task Save(CancellationToken cancellationToken)
		{
			foreach (var song in GetUpdatedSongs())
			{
				await songsService.UpdateSong(song, UpdatedSongPropertiesModel.ForceTagUpdate, cancellationToken);
			}
		}

		private static IReadOnlyCollection<EditedSongProperty<T>> GetAvailableListItems<T>(IEnumerable<T> values)
			where T : class
		{
			var availableValues = new List<EditedSongProperty<T>>
			{
				new EditedSongProperty<T>(),
				new EditedSongProperty<T>(null),
			};

			availableValues.AddRange(values.Select(v => new EditedSongProperty<T>(v)));

			return availableValues;
		}

		private EditedSongProperty<T> SelectCurrentListItem<T>(IEnumerable<EditedSongProperty<T>> listItems, Func<SongModel, T> propertySelector, IEqualityComparer<T> comparer)
			where T : class
		{
			var distinctValue = GetDistinctValue(editedSongs, propertySelector, comparer);
			foreach (var item in listItems)
			{
				if ((distinctValue == null && !item.HasValue) ||
				    (distinctValue != null && item.HasValue && comparer.Equals(distinctValue, item.Value)))
				{
					return item;
				}
			}

			throw new InvalidOperationException("Failed to select current item in the list");
		}

		private IEnumerable<SongModel> GetUpdatedSongs()
		{
			foreach (var song in editedSongs)
			{
				if (SingleSongMode)
				{
					song.Title = Title;
					song.TreeTitle = TreeTitle;
				}

				if (Artist.HasValue)
				{
					song.Artist = Artist.Value;
				}

				if (Genre.HasValue)
				{
					song.Genre = Genre.Value;
				}

				if (TrackNumber.HasValue)
				{
					song.TrackNumber = TrackNumber.Value;
				}

				yield return song;
			}
		}

		private static EditedSongProperty<T> BuildProperty<T>(IEnumerable<SongModel> songs, Func<SongModel, T> propertySelector, IEqualityComparer<T> comparer)
		{
			var value = GetDistinctValue(songs, propertySelector, comparer);
			return value != null ? new EditedSongProperty<T>(value) : new EditedSongProperty<T>();
		}

		private static T GetDistinctValue<T>(IEnumerable<SongModel> songs, Func<SongModel, T> propertySelector, IEqualityComparer<T> comparer)
		{
			var propertyValues = songs.Select(propertySelector).Distinct(comparer).ToList();
			return propertyValues.Count == 1 ? propertyValues.Single() : default;
		}
	}
}
