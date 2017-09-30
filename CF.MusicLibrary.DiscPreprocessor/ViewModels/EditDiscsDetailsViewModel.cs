using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CF.Library.Core.Exceptions;
using CF.Library.Core.Facades;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.DiscPreprocessor.AddingToLibrary;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;
using CF.MusicLibrary.DiscPreprocessor.ViewModels.Interfaces;
using GalaSoft.MvvmLight;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.DiscPreprocessor.ViewModels
{
	public class EditDiscsDetailsViewModel : ViewModelBase, IEditDiscsDetailsViewModel
	{
		private const string CoverImageFileName = "cover.jpg";

		private readonly DiscLibrary discLibrary;
		private readonly ILibraryStructurer libraryStructurer;
		private readonly IFileSystemFacade fileSystemFacade;

		public string Name => "Edit Discs Details";

		public bool DataIsReady => Discs.All(a => a.RequiredDataIsFilled);

		public ObservableCollection<DiscViewItem> Discs { get; private set; }

		public IEnumerable<AddedDiscCoverImage> DiscCoverImages
		{
			get
			{
				foreach (DiscViewItem disc in Discs)
				{
					var coverImagePath = Path.Combine(disc.SourcePath, CoverImageFileName);
					if (fileSystemFacade.FileExists(coverImagePath))
					{
						yield return new AddedDiscCoverImage(disc.Disc, coverImagePath);
					}
				}
			}
		}

		public IEnumerable<AddedSong> AddedSongs
		{
			get
			{
				foreach (var addedDisc in Discs)
				{
					foreach (var addedSong in addedDisc.Songs)
					{
						addedSong.Song.Uri = libraryStructurer.BuildSongUri(addedDisc.Disc.Uri, Path.GetFileName(addedSong.SourceFileName));
						yield return addedSong;
					}
				}
			}
		}

		public EditDiscsDetailsViewModel(DiscLibrary discLibrary, ILibraryStructurer libraryStructurer, IFileSystemFacade fileSystemFacade)
		{
			if (discLibrary == null)
			{
				throw new ArgumentNullException(nameof(discLibrary));
			}
			if (libraryStructurer == null)
			{
				throw new ArgumentNullException(nameof(libraryStructurer));
			}
			if (fileSystemFacade == null)
			{
				throw new ArgumentNullException(nameof(fileSystemFacade));
			}

			this.discLibrary = discLibrary;
			this.libraryStructurer = libraryStructurer;
			this.fileSystemFacade = fileSystemFacade;
		}

		public async Task SetDiscs(IEnumerable<AddedDiscInfo> discs)
		{
			await discLibrary.Load();

			var discsList = discs.ToList();
			var availableArtists = BuildAvailableArtistsList(discsList);

			Discs = new ObservableCollection<DiscViewItem>();
			foreach (var addedDiscInfo in discsList)
			{
				DiscViewItem addedDisc;
				switch (addedDiscInfo.DiscType)
				{
					case DsicType.ArtistDisc:
						addedDisc = new ArtistDiscViewItem(addedDiscInfo.SourcePath, addedDiscInfo, availableArtists, discLibrary.Genres, PredictArtistGenre(addedDiscInfo.Artist));
						break;

					case DsicType.CompilationDiscWithArtistInfo:
						addedDisc = new CompilationDiscWithArtistInfoViewItem(addedDiscInfo.SourcePath, addedDiscInfo, availableArtists, discLibrary.Genres);
						break;

					case DsicType.CompilationDiscWithoutArtistInfo:
						addedDisc = new CompilationDiscWithoutArtistInfoViewItem(addedDiscInfo.SourcePath, addedDiscInfo, availableArtists, discLibrary.Genres);
						break;

					default:
						throw new UnexpectedEnumValueException(addedDiscInfo.DiscType);
				}

				addedDisc.PropertyChanged += Property_Changed;
				Discs.Add(addedDisc);
			}
		}

		private List<Artist> BuildAvailableArtistsList(IEnumerable<AddedDiscInfo> discs)
		{
			List<Artist> artists = discLibrary.AllArtists.ToList();

			var discsList = discs.ToList();
			foreach (var songArtistName in discsList.Where(d => d.HasArtist).Select(d => d.Artist)
				//	We're adding Song artists even if disc.HasArtist is true,
				//	so that individual song artists are also get into artist list.
				.Concat(discsList.SelectMany(d => d.Songs).Select(s => s.Artist).Where(a => !String.IsNullOrEmpty(a)))
				.Distinct())
			{
				var matchedArtist = artists.SingleOrDefault(a => String.Equals(a.Name, songArtistName, StringComparison.Ordinal));
				var matchedArtistCaseInsensitive = artists.SingleOrDefault(a => String.Equals(a.Name, songArtistName, StringComparison.OrdinalIgnoreCase));

				if (matchedArtist == null && matchedArtistCaseInsensitive != null)
				{
					throw new InvalidOperationException(Current($"Artist name differs only by case: '{songArtistName}'"));
				}

				if (matchedArtist == null)
				{
					artists.Add(new Artist
					{
						Name = songArtistName
					});
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

			ArtistDiscViewItem changedDisc = sender as ArtistDiscViewItem;
			if (changedDisc != null)
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
				ArtistDiscViewItem artistDisc = disc as ArtistDiscViewItem;
				if (artistDisc != null && valueIsEmpty(artistDisc))
				{
					setValue(artistDisc);
				}
			}
		}

		private Genre PredictArtistGenre(string artist)
		{
			//	Selecting genre of the most recent disc
			return discLibrary
				.OrderByDescending(d => d.Year)
				.Where(d => d.Artist?.Name == artist)
				.Where(d => d.Genre != null)
				.Select(d => d.Genre).FirstOrDefault();
		}
	}
}
