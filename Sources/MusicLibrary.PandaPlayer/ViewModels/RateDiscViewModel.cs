using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MusicLibrary.Core.Objects;
using MusicLibrary.PandaPlayer.ContentUpdate;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace MusicLibrary.PandaPlayer.ViewModels
{
	public class RateDiscViewModel : ViewModelBase, IRateDiscViewModel
	{
		private readonly ILibraryContentUpdater libraryContentUpdater;

		private Disc Disc { get; set; }

		public string DiscTitle => Disc.Title;

		public static IEnumerable<Rating> AvailableRatings => RatingsHelper.AllowedRatingsDesc;

		private Rating selectedRating;

		public Rating SelectedRating
		{
			get => selectedRating;
			set => Set(ref selectedRating, value);
		}

		public RateDiscViewModel(ILibraryContentUpdater libraryContentUpdater)
		{
			this.libraryContentUpdater = libraryContentUpdater ?? throw new ArgumentNullException(nameof(libraryContentUpdater));
		}

		public void Load(Disc disc)
		{
			Disc = disc;
			SelectedRating = Song.DefaultRating;
		}

		public async Task Save()
		{
			await libraryContentUpdater.SetSongsRating(Disc.Songs, SelectedRating);
		}
	}
}
