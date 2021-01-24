using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MusicLibrary.Core.Models;
using MusicLibrary.DiscAdder.AddedContent;
using MusicLibrary.DiscAdder.Extensions;
using MusicLibrary.DiscAdder.Interfaces;
using MusicLibrary.DiscAdder.MusicStorage;
using MusicLibrary.DiscAdder.ViewModels.Interfaces;
using MusicLibrary.DiscAdder.ViewModels.ViewModelItems;
using MusicLibrary.Services.Interfaces;

namespace MusicLibrary.DiscAdder.ViewModels
{
	internal class EditDiscsDetailsViewModel : ViewModelBase, IEditDiscsDetailsViewModel
	{
		private readonly IFolderProvider folderProvider;

		private readonly IDiscsService discService;

		private readonly IArtistsService artistService;

		private readonly IGenresService genreService;

		public string Name => "Edit Discs Details";

		public bool DataIsReady => Discs.All(a => a.RequiredDataIsFilled);

		public ObservableCollection<DiscViewItem> Discs { get; }

		private IEnumerable<(DiscViewItem DiscItem, AddedDisc AddedDisc)> DiscPairs => Discs.Select(d => (d, new AddedDisc(d.Disc, d is NewDiscViewItem, d.SourcePath, d.DestinationFolderPath)));

		public IEnumerable<AddedDisc> AddedDiscs => DiscPairs.Select(p => p.AddedDisc);

		public IEnumerable<AddedSong> AddedSongs => DiscPairs.SelectMany(p => p.DiscItem.Songs.Select(s => new AddedSong(p.AddedDisc, s.Song, s.SourcePath)));

		public EditDiscsDetailsViewModel(IFolderProvider folderProvider, IDiscsService discService, IArtistsService artistService, IGenresService genreService)
		{
			this.folderProvider = folderProvider ?? throw new ArgumentNullException(nameof(folderProvider));
			this.discService = discService ?? throw new ArgumentNullException(nameof(discService));
			this.artistService = artistService ?? throw new ArgumentNullException(nameof(artistService));
			this.genreService = genreService ?? throw new ArgumentNullException(nameof(genreService));
			Discs = new ObservableCollection<DiscViewItem>();
		}

		public async Task SetDiscs(IEnumerable<AddedDiscInfo> discs, CancellationToken cancellationToken)
		{
			var discsList = discs.ToList();
			var libraryArtists = await artistService.GetAllArtists(cancellationToken);
			var availableGenres = await genreService.GetAllGenres(cancellationToken);

			// For genre guessing we use all discs, including deleted.
			var allDiscs = await discService.GetAllDiscs(cancellationToken);

			Discs.Clear();
			foreach (var addedDiscInfo in discsList)
			{
				var parentFolder = await folderProvider.GetFolder(addedDiscInfo.DestinationFolderPath, cancellationToken);
				var folderExists = parentFolder != null;

				var existingDisc = parentFolder?.Discs.SingleOrDefault(d => d.TreeTitle == addedDiscInfo.TreeTitle);

				var availableArtists = BuildAvailableArtistsList(addedDiscInfo, libraryArtists);

				var addedDisc = existingDisc != null ?
					new ExistingDiscViewItem(existingDisc, addedDiscInfo, availableArtists, availableGenres) as DiscViewItem :
					new NewDiscViewItem(addedDiscInfo, folderExists, availableArtists, availableGenres, GuessArtistGenre(allDiscs, addedDiscInfo.Artist));

				addedDisc.PropertyChanged += Property_Changed;
				Discs.Add(addedDisc);
			}
		}

		private static IEnumerable<ArtistViewItem> BuildAvailableArtistsList(AddedDiscInfo disc, IEnumerable<ArtistModel> libraryArtists)
		{
			var artists = new List<ArtistViewItem>
			{
				new EmptyArtistViewItem(),
			};

			if (disc.HasVariousArtists)
			{
				artists.Add(new VariousArtistViewItem());
			}

			var specificArtists = new List<ArtistModel>(libraryArtists);

			var newDiscArtistNames = new[] { disc.Artist }
				.Concat(disc.Songs.Select(s => s.Artist))
				.Where(a => !String.IsNullOrEmpty(a))
				.Distinct();

			foreach (var songArtistName in newDiscArtistNames)
			{
				var matchedArtist = specificArtists.SingleOrDefault(a => String.Equals(a.Name, songArtistName, StringComparison.Ordinal));
				if (matchedArtist != null)
				{
					continue;
				}

				var matchedArtistCaseInsensitive = specificArtists.SingleOrDefault(a => String.Equals(a.Name, songArtistName, StringComparison.OrdinalIgnoreCase));
				if (matchedArtistCaseInsensitive != null)
				{
					throw new InvalidOperationException($"Artist name differs only by case: '{songArtistName}'");
				}

				var newArtist = new ArtistModel
				{
					Name = songArtistName,
				};

				specificArtists.Add(newArtist);
			}

			var specificArtistItems = specificArtists
				.OrderBy(a => a.Name)
				.Select(a => new SpecificArtistViewItem(a));

			artists.AddRange(specificArtistItems);

			return artists;
		}

		private void Property_Changed(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(DiscViewItem.RequiredDataIsFilled))
			{
				RaisePropertyChanged(nameof(DataIsReady));
				return;
			}

			if (sender is NewDiscViewItem changedDisc && e.PropertyName == nameof(DiscViewItem.Genre) && changedDisc.Artist is SpecificArtistViewItem specificArtist1)
			{
				foreach (var disc in Discs.Where(d => d != changedDisc && d.Artist is SpecificArtistViewItem specificArtist2 && specificArtist1.Equals(specificArtist2)))
				{
					if (disc is NewDiscViewItem sameArtistDisc && sameArtistDisc.Genre == null)
					{
						sameArtistDisc.Genre = changedDisc.Genre;
					}
				}
			}
		}

		private static GenreModel GuessArtistGenre(IEnumerable<DiscModel> discs, string artist)
		{
			if (artist == null)
			{
				return null;
			}

			// Selecting genre from the most recent disc of the same artist.
			return discs
				.Where(d => d.SoloArtist?.Name == artist)
				.OrderByDescending(d => d.Year)
				.Select(d => d.GetGenre())
				.FirstOrDefault(g => g != null);
		}
	}
}
