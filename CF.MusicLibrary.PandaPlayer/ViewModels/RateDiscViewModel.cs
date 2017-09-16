using System.Collections.Generic;
using CF.MusicLibrary.BL.Objects;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class RateDiscViewModel : ViewModelBase
	{
		public string DiscTitle { get; }

		public static IEnumerable<Rating> AvailableRatings => RatingsHelper.AllowedRatingsDesc;

		private Rating? selectedRating;
		public Rating? SelectedRating
		{
			get { return selectedRating;}
			set { Set(ref selectedRating, value); }
		}

		public RateDiscViewModel(Disc disc)
		{
			DiscTitle = disc.Title;
			SelectedRating = Song.DefaultRating;
		}
	}
}
