using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Wpf;
using MusicLibrary.Logic.Models;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace MusicLibrary.PandaPlayer.ViewModels
{
	public class SetRatingMenuItem
	{
		private readonly ISongListViewModel parentViewModel;

		public RatingModel Rating { get; }

		public ICommand Command { get; }

		public SetRatingMenuItem(ISongListViewModel parentViewModel, RatingModel rating)
		{
			this.parentViewModel = parentViewModel ?? throw new ArgumentNullException(nameof(parentViewModel));
			this.Rating = rating;
			Command = new AsyncRelayCommand(() => SetRating(rating, CancellationToken.None));
		}

		private async Task SetRating(RatingModel rating, CancellationToken cancellationToken)
		{
			await parentViewModel.SetRatingForSelectedSongs(rating, cancellationToken);
		}
	}
}
