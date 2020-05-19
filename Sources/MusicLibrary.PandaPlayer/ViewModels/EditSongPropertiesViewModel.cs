﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MusicLibrary.Core;
using MusicLibrary.Core.Interfaces;
using MusicLibrary.Core.Objects;
using MusicLibrary.Logic.Interfaces.Services;
using MusicLibrary.Logic.Models;
using MusicLibrary.PandaPlayer.ContentUpdate;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace MusicLibrary.PandaPlayer.ViewModels
{
	public class EditSongPropertiesViewModel : ViewModelBase, IEditSongPropertiesViewModel
	{
		private readonly IGenresService genresService;
		private readonly IArtistsService artistsService;

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
			get => fileName;
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

				return String.Equals(FileName, GetSongFileName(SingleSong), StringComparison.Ordinal) ?
					null : libraryStructurer.BuildSongUri(SingleSong.Disc.Uri, FileName);
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

		public EditedSongProperty<Artist> Artist { get; set; }

		public EditedSongProperty<Genre> Genre { get; set; }

		public EditedSongProperty<short?> Year { get; set; }

		public EditedSongProperty<short?> TrackNumber { get; set; }

		public IReadOnlyCollection<EditedSongProperty<ArtistModel>> AvailableArtists { get; private set; }

		public IReadOnlyCollection<EditedSongProperty<GenreModel>> AvailableGenres { get; private set; }

		public EditSongPropertiesViewModel(IGenresService genresService, IArtistsService artistsService, ILibraryStructurer libraryStructurer, ILibraryContentUpdater libraryContentUpdater)
		{
			this.genresService = genresService ?? throw new ArgumentNullException(nameof(genresService));
			this.artistsService = artistsService ?? throw new ArgumentNullException(nameof(artistsService));
			this.libraryStructurer = libraryStructurer ?? throw new ArgumentNullException(nameof(libraryStructurer));
			this.libraryContentUpdater = libraryContentUpdater ?? throw new ArgumentNullException(nameof(libraryContentUpdater));
		}

		public async Task Load(IEnumerable<Song> songs, CancellationToken cancellationToken)
		{
			editedSongs = songs.ToList();
			if (editedSongs.Count == 0)
			{
				throw new InvalidOperationException("Songs list is empty");
			}

			AvailableArtists = await FillAvailableValues(() => artistsService.GetAllArtists(cancellationToken));
			AvailableGenres = await FillAvailableValues(() => genresService.GetAllGenres(cancellationToken));

			fileName = SingleSongMode ? GetSongFileName(SingleSong) : null;
			title = SingleSongMode ? SingleSong.Title : null;
			Artist = BuildProperty(editedSongs, s => s.Artist);
			Genre = BuildProperty(editedSongs, s => s.Genre);
			Year = BuildProperty(editedSongs, s => s.Year);
			TrackNumber = BuildProperty(editedSongs, s => s.TrackNumber);
		}

		public async Task Save()
		{
			// Should we rename a song?
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
