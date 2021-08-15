using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CodeFuller.Library.Wpf;
using PandaPlayer.Core.Models;

namespace PandaPlayer.ViewModels.MenuItems
{
	public class SetRatingMenuItem
	{
		public RatingModel Rating { get; }

		public ICommand Command { get; }

		public SetRatingMenuItem(Func<RatingModel, CancellationToken, Task> setRatingAction, RatingModel rating)
		{
			Rating = rating;
			Command = new AsyncRelayCommand(() => setRatingAction(rating, CancellationToken.None));
		}
	}
}
