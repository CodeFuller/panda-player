using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PandaPlayer.Core.Models;
using PandaPlayer.DiscAdder.Extensions;
using PandaPlayer.DiscAdder.Interfaces;
using PandaPlayer.DiscAdder.MusicStorage;
using PandaPlayer.DiscAdder.ViewModels.Interfaces;
using PandaPlayer.DiscAdder.ViewModels.ViewModelItems;
using PandaPlayer.Services.Interfaces;

namespace PandaPlayer.DiscAdder.ViewModels
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
			ClearDiscItems();

			var discsList = discs.ToList();
			var libraryArtists = await artistService.GetAllArtists(cancellationToken);
			var availableGenres = await genreService.GetAllGenres(cancellationToken);

			// For genre guessing we use all discs, including deleted.
			var allDiscs = await discService.GetAllDiscs(cancellationToken);

			foreach (var addedDiscInfo in discsList)
			{
				var parentFolder = await folderProvider.GetFolder(addedDiscInfo.DestinationFolderPath, cancellationToken);
				var folderExists = parentFolder != null;

				var existingDisc = parentFolder?.Discs.SingleOrDefault(d => d.TreeTitle == addedDiscInfo.TreeTitle);

				var availableArtists = BuildAvailableArtistsList(addedDiscInfo, libraryArtists);

				DiscViewItem discViewItem = existingDisc != null ?
					new ExistingDiscViewItem(existingDisc, addedDiscInfo, availableArtists, availableGenres) :
					new NewDiscViewItem(addedDiscInfo, folderExists, availableArtists, availableGenres, GuessArtistGenre(allDiscs, addedDiscInfo.Artist));

				discViewItem.PropertyChanged += DiscItem_PropertyChanged;
				Discs.Add(discViewItem);
			}
		}

		private void ClearDiscItems()
		{
			foreach (var discItem in Discs)
			{
				discItem.PropertyChanged -= DiscItem_PropertyChanged;
			}

			Discs.Clear();
		}

		private static IEnumerable<BasicInputArtistItem> BuildAvailableArtistsList(AddedDiscInfo disc, IEnumerable<ArtistModel> libraryArtists)
		{
			var artists = new List<BasicInputArtistItem>
			{
				new EmptyInputArtistItem(),
			};

			if (disc.HasVariousArtists)
			{
				artists.Add(new VariousInputArtistItem());
			}

			var specificArtists = libraryArtists
				.ToDictionary(x => x.Name, x => new SpecificInputArtistItem(x.Name, isNewArtist: false), StringComparer.OrdinalIgnoreCase);

			var newDiscArtistNames = new[] { disc.Artist }
				.Concat(disc.Songs.Select(s => s.Artist))
				.Where(a => !String.IsNullOrEmpty(a))
				.Distinct();

			foreach (var songArtistName in newDiscArtistNames)
			{
				if (specificArtists.TryGetValue(songArtistName, out var existingArtist))
				{
					if (existingArtist.ArtistName != songArtistName)
					{
						throw new InvalidOperationException($"Artist name differs only by case: '{songArtistName}'");
					}

					continue;
				}

				var newArtist = new SpecificInputArtistItem(songArtistName, isNewArtist: true);
				specificArtists.Add(songArtistName, newArtist);
			}

			artists.AddRange(specificArtists.Values.OrderBy(a => a.ArtistName));

			return artists;
		}

		private void DiscItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(DiscViewItem.RequiredDataIsFilled))
			{
				RaisePropertyChanged(nameof(DataIsReady));
				return;
			}

			if (sender is NewDiscViewItem changedDisc && e.PropertyName == nameof(DiscViewItem.Genre) && changedDisc.Artist is SpecificInputArtistItem specificArtist1)
			{
				// Updating genre for all other discs of the same artist.
				foreach (var disc in Discs.Where(d => d != changedDisc && d.Artist is SpecificInputArtistItem specificArtist2 && specificArtist1.Equals(specificArtist2)))
				{
					if (disc is NewDiscViewItem { Genre: null } sameArtistDisc)
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
