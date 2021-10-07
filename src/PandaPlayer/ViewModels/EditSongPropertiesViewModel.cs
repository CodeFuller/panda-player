﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PandaPlayer.Core.Comparers;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels
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

		private short? trackNumber;

		public short? TrackNumber
		{
			get => trackNumber;
			set
			{
				if (!SingleSongMode)
				{
					throw new InvalidOperationException("Track number could not be edited in multi-song mode");
				}

				Set(ref trackNumber, value);
			}
		}

		public EditedSongProperty<ArtistModel> Artist { get; set; }

		public string NewArtistName { get; set; }

		public EditedSongProperty<GenreModel> Genre { get; set; }

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
			trackNumber = SingleSongMode ? SingleSong.TrackNumber : null;
		}

		private async Task LoadArtists(CancellationToken cancellationToken)
		{
			var allArtists = await artistsService.GetAllArtists(cancellationToken);
			Artist = BuildProperty(editedSongs, s => s.Artist, new ArtistEqualityComparer());
			AvailableArtists = GetAvailableListItems(allArtists, Artist, new ArtistEqualityComparer());
		}

		private async Task LoadGenres(CancellationToken cancellationToken)
		{
			var allGenres = await genresService.GetAllGenres(cancellationToken);
			Genre = BuildProperty(editedSongs, s => s.Genre, new GenreEqualityComparer());
			AvailableGenres = GetAvailableListItems(allGenres, Genre, new GenreEqualityComparer());
		}

		public async Task Save(CancellationToken cancellationToken)
		{
			var songs = editedSongs;
			await UpdateSongModels(songs, cancellationToken);

			foreach (var song in songs)
			{
				await songsService.UpdateSong(song, cancellationToken);
			}
		}

		private static IReadOnlyCollection<EditedSongProperty<T>> GetAvailableListItems<T>(IEnumerable<T> values, EditedSongProperty<T> selectedItem, IEqualityComparer<T> comparer)
			where T : class
		{
			var availableItems = new List<EditedSongProperty<T>>();

			// We add <keep> item only if it must be selected.
			if (!selectedItem.HasValue)
			{
				availableItems.Add(selectedItem);
			}

			// <blank> item
			availableItems.Add(selectedItem.HasBlankValue ? selectedItem : new EditedSongProperty<T>(null));

			foreach (var value in values)
			{
				// We must select the same EditedSongProperty instance in the list.
				var item = selectedItem.HasValue && comparer.Equals(value, selectedItem.Value) ? selectedItem : new EditedSongProperty<T>(value);
				availableItems.Add(item);
			}

			return availableItems;
		}

		private async Task UpdateSongModels(IEnumerable<SongModel> songs, CancellationToken cancellationToken)
		{
			// Possible cases:
			//   1. NewArtistName == null: User has not typed new artist name (artist was left unchanged or existing artist from the list was selected).
			//   2. NewArtistName != null && Artist == null: User has typed new artist name.
			//   3. NewArtistName != null && Artist != null && NewArtistName == Artist.Name: User has typed existing artist name.
			//   4. NewArtistName != null && Artist != null && NewArtistName != Artist.Name: User has typed new artist name which partially matches existing artist name,
			//      e.g. NewArtistName == "Metallica" while "Metallica & Nirvana" exists in the list.
			//
			// We are creating new artist for the cases #2 and #4.
			ArtistModel newArtist = null;
			if (!String.IsNullOrEmpty(NewArtistName) && (Artist == null || NewArtistName != Artist.Value.Name))
			{
				newArtist = new ArtistModel
				{
					Name = NewArtistName,
				};

				await artistsService.CreateArtist(newArtist, cancellationToken);
			}

			foreach (var song in songs)
			{
				if (SingleSongMode)
				{
					song.Title = Title;
					song.TreeTitle = TreeTitle;
					song.TrackNumber = TrackNumber;
				}

				if (newArtist != null)
				{
					song.Artist = newArtist;
				}
				else if (Artist.HasValue)
				{
					song.Artist = Artist.Value;
				}

				if (Genre.HasValue)
				{
					song.Genre = Genre.Value;
				}
			}
		}

		private static EditedSongProperty<T> BuildProperty<T>(IEnumerable<SongModel> songs, Func<SongModel, T> propertySelector, IEqualityComparer<T> comparer)
		{
			var propertyValues = songs.Select(propertySelector).Distinct(comparer).ToList();
			return propertyValues.Count == 1 ? new EditedSongProperty<T>(propertyValues.Single()) : new EditedSongProperty<T>();
		}
	}
}
