using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MusicLibrary.Logic.Interfaces.Services;
using MusicLibrary.Logic.Models;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;

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

			AvailableArtists = await FillAvailableValues(() => artistsService.GetAllArtists(cancellationToken));
			AvailableGenres = await FillAvailableValues(() => genresService.GetAllGenres(cancellationToken));

			treeTitle = SingleSongMode ? SingleSong.TreeTitle : null;
			title = SingleSongMode ? SingleSong.Title : null;

			var distinctArtistId = GetDistinctValue(editedSongs, s => s.ArtistId);
			Artist = distinctArtistId != null ? AvailableArtists.Single(a => a.Value.Id == distinctArtistId) : null;

			var distinctGenreId = GetDistinctValue(editedSongs, s => s.GenreId);
			Genre = distinctGenreId != null ? AvailableGenres.Single(g => g.Value.Id == distinctGenreId) : null;

			TrackNumber = BuildProperty(editedSongs, s => s.TrackNumber);
		}

		public async Task Save(CancellationToken cancellationToken)
		{
			foreach (var song in GetUpdatedSongs())
			{
				await songsService.UpdateSong(song, UpdatedSongPropertiesModel.ForceTagUpdate, cancellationToken);
			}
		}

		private static async Task<IReadOnlyCollection<EditedSongProperty<T>>> FillAvailableValues<T>(Func<Task<IReadOnlyCollection<T>>> valuesProvider)
			where T : class
		{
			var availableValues = new List<EditedSongProperty<T>>
			{
				new EditedSongProperty<T>(),
				new EditedSongProperty<T>(null),
			};

			var loadedValues = await valuesProvider();
			availableValues.AddRange(loadedValues.Select(v => new EditedSongProperty<T>(v)));

			return availableValues;
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

		private static EditedSongProperty<T> BuildProperty<T>(IEnumerable<SongModel> songs, Func<SongModel, T> propertyAccessor)
		{
			var propertyValues = songs.Select(propertyAccessor).Distinct().ToList();
			return propertyValues.Count == 1 ? new EditedSongProperty<T>(propertyValues.Single()) : new EditedSongProperty<T>();
		}

		private static T GetDistinctValue<T>(IEnumerable<SongModel> songs, Func<SongModel, T> propertyAccessor)
			where T : class
		{
			var propertyValues = songs.Select(propertyAccessor).Distinct().ToList();
			return propertyValues.Count == 1 ? propertyValues.Single() : null;
		}
	}
}
