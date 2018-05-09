using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
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
			get { return selectedRating;}
			set { Set(ref selectedRating, value); }
		}

		public RateDiscViewModel(ILibraryContentUpdater libraryContentUpdater)
		{
			if (libraryContentUpdater == null)
			{
				throw new ArgumentNullException(nameof(libraryContentUpdater));
			}

			this.libraryContentUpdater = libraryContentUpdater;
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
