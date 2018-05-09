using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Wpf;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class SetRatingMenuItem
	{
		private readonly ISongListViewModel parentViewModel;

		public Rating Rating { get; }

		public ICommand Command { get; }

		public SetRatingMenuItem(ISongListViewModel parentViewModel, Rating rating)
		{
			this.parentViewModel = parentViewModel ?? throw new ArgumentNullException(nameof(parentViewModel));
			this.Rating = rating;
			Command = new AsyncRelayCommand(() => SetRating(rating));
		}

		private async Task SetRating(Rating rating)
		{
			await parentViewModel.SetRatingForSelectedSongs(rating);
		}
	}
}
