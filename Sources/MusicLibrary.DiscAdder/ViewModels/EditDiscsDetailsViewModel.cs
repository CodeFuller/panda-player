using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CF.Library.Core.Exceptions;
using GalaSoft.MvvmLight;
using MusicLibrary.Core.Models;
using MusicLibrary.DiscAdder.AddingToLibrary;
using MusicLibrary.DiscAdder.Extensions;
using MusicLibrary.DiscAdder.Interfaces;
using MusicLibrary.DiscAdder.MusicStorage;
using MusicLibrary.DiscAdder.ViewModels.Interfaces;
using MusicLibrary.Services.Interfaces;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

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

		public IEnumerable<AddedDisc> AddedDiscs => Discs.Select(d => new AddedDisc(d.Disc, d is NewDiscViewItem, d.SourcePath));

		public IEnumerable<AddedSong> AddedSongs => Discs.SelectMany(addedDisc => addedDisc.Songs);

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
			var availableArtists = await BuildAvailableArtistsList(discsList, cancellationToken);
			var availableGenres = await genreService.GetAllGenres(cancellationToken);

			// For genre guessing we use all discs, including deleted.
			var allDiscs = await discService.GetAllDiscs(cancellationToken);

			Discs.Clear();
			foreach (var addedDiscInfo in discsList)
			{
				var parentFolder = await folderProvider.GetFolder(addedDiscInfo.DestinationFolderPath, cancellationToken);
				var folderExists = parentFolder != null;

				var existingDisc = parentFolder?.Discs.SingleOrDefault(d => d.TreeTitle == addedDiscInfo.TreeTitle);

				DiscViewItem addedDisc;

				if (existingDisc != null)
				{
					addedDisc = new ExistingDiscViewItem(existingDisc, addedDiscInfo, availableArtists, availableGenres);
				}
				else
				{
					switch (addedDiscInfo.DiscType)
					{
						case DiscType.ArtistDisc:
							addedDisc = new ArtistDiscViewItem(addedDiscInfo, folderExists, availableArtists, availableGenres, PredictArtistGenre(allDiscs, addedDiscInfo.Artist));
							break;

						case DiscType.CompilationDiscWithArtistInfo:
							addedDisc = new CompilationDiscWithArtistInfoViewItem(addedDiscInfo, folderExists, availableArtists, availableGenres);
							break;

						case DiscType.CompilationDiscWithoutArtistInfo:
							addedDisc = new CompilationDiscWithoutArtistInfoViewItem(addedDiscInfo, folderExists, availableArtists, availableGenres);
							break;

						default:
							throw new UnexpectedEnumValueException(addedDiscInfo.DiscType);
					}
				}

				addedDisc.PropertyChanged += Property_Changed;
				Discs.Add(addedDisc);
			}
		}

		private async Task<IReadOnlyCollection<ArtistModel>> BuildAvailableArtistsList(IEnumerable<AddedDiscInfo> discs, CancellationToken cancellationToken)
		{
			var libraryArtists = await artistService.GetAllArtists(cancellationToken);
			var artists = new List<ArtistModel>(libraryArtists);

			var discsList = discs.ToList();

			// We're adding Song artists even if disc.HasArtist is true, so that individual song artists also get into the artist list.
			var newDiscsArtistNames = discsList.Where(d => d.HasArtist)
				.Select(d => d.Artist)
				.Concat(discsList.SelectMany(d => d.Songs).Select(s => s.Artist).Where(a => !String.IsNullOrEmpty(a)))
				.Distinct();

			foreach (var songArtistName in newDiscsArtistNames)
			{
				var matchedArtist = artists.SingleOrDefault(a => String.Equals(a.Name, songArtistName, StringComparison.Ordinal));
				var matchedArtistCaseInsensitive = artists.SingleOrDefault(a => String.Equals(a.Name, songArtistName, StringComparison.OrdinalIgnoreCase));

				if (matchedArtist == null && matchedArtistCaseInsensitive != null)
				{
					throw new InvalidOperationException(Current($"Artist name differs only by case: '{songArtistName}'"));
				}

				if (matchedArtist == null)
				{
					var newArtist = new ArtistModel
					{
						Name = songArtistName,
					};

					artists.Add(newArtist);
				}
			}

			return artists;
		}

		private void Property_Changed(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(DiscViewItem.RequiredDataIsFilled))
			{
				RaisePropertyChanged(nameof(DataIsReady));
			}

			if (sender is ArtistDiscViewItem changedDisc)
			{
				if (e.PropertyName == nameof(DiscViewItem.Genre))
				{
					FillSameArtistDiscsData(changedDisc, a => a.Genre == null, a => a.Genre = changedDisc.Genre);
				}
			}
		}

		private void FillSameArtistDiscsData(ArtistDiscViewItem changedDisc, Func<ArtistDiscViewItem, bool> valueIsEmpty, Action<ArtistDiscViewItem> setValue)
		{
			foreach (var disc in Discs.Where(a => a != changedDisc && a.Artist == changedDisc.Artist))
			{
				if (disc is ArtistDiscViewItem artistDisc && valueIsEmpty(artistDisc))
				{
					setValue(artistDisc);
				}
			}
		}

		private static GenreModel PredictArtistGenre(IEnumerable<DiscModel> discs, string artist)
		{
			// Selecting genre of the most recent disc
			return discs
				.Where(d => d.SoloArtist?.Name == artist)
				.OrderByDescending(d => d.Year)
				.Select(d => d.GetGenre())
				.FirstOrDefault(g => g != null);
		}
	}
}
