using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CodeFuller.Library.Wpf;
using CodeFuller.Library.Wpf.Interfaces;
using PandaPlayer.Core.Models;
using PandaPlayer.DiscAdder.Views;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.Views;

namespace PandaPlayer
{
	public class ViewNavigator : IViewNavigator
	{
		private static Window ApplicationWindow => Application.Current.MainWindow;

		private readonly INavigatedViewModelHolder viewModelHolder;

		private readonly IWindowService windowService;

		public ViewNavigator(INavigatedViewModelHolder viewModelHolder, IWindowService windowService)
		{
			this.viewModelHolder = viewModelHolder ?? throw new ArgumentNullException(nameof(viewModelHolder));
			this.windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));
		}

		public void ShowRatePlaylistSongsView(IEnumerable<SongModel> songs)
		{
			var songsList = songs.ToList();
			var unratedSongsNumber = songsList.Count(s => s.Rating == null);
			if (unratedSongsNumber == 0)
			{
				throw new InvalidOperationException("All disc songs are already rated");
			}

			if (unratedSongsNumber == songsList.Count)
			{
				var viewModel = viewModelHolder.RateSongsViewModel;
				viewModel.Load(songsList);
				ShowDialog<RateSongsView>(viewModel);
			}
			else
			{
				var message = "You've just finished listening of playlist that have some songs unrated. Please devote some time and rate them.";
				windowService.ShowMessageBox(message, "Rate listened songs", ShowMessageBoxButton.Ok, ShowMessageBoxIcon.Exclamation);
			}
		}

		public void ShowDiscPropertiesView(DiscModel disc)
		{
			var viewModel = viewModelHolder.EditDiscPropertiesViewModel;
			viewModel.Load(disc);
			ShowDialog<EditDiscPropertiesView>(viewModel);
		}

		public async Task ShowSongPropertiesView(IEnumerable<SongModel> songs, CancellationToken cancellationToken)
		{
			var viewModel = viewModelHolder.EditSongPropertiesViewModel;
			await viewModel.Load(songs, cancellationToken);
			ShowDialog<EditSongPropertiesView>(viewModel);
		}

		public void ShowEditDiscImageView(DiscModel disc)
		{
			var viewModel = viewModelHolder.EditDiscImageViewModel;
			viewModel.Load(disc);
			ShowDialog<EditDiscImageView>(viewModel);
		}

		public async Task ShowDiscAdderView(CancellationToken cancellationToken)
		{
			var viewModel = viewModelHolder.DiscAdderViewModel;
			await viewModel.Load(cancellationToken);

			ShowView<DiscAdderView>(viewModel);
		}

		public Task ShowLibraryCheckerView(CancellationToken cancellationToken)
		{
			ShowView<LibraryCheckerView>(viewModelHolder.LibraryCheckerViewModel);

			return Task.CompletedTask;
		}

		public async Task ShowLibraryStatisticsView(CancellationToken cancellationToken)
		{
			var viewModel = viewModelHolder.LibraryStatisticsViewModel;
			await viewModel.Load(cancellationToken);
			ShowDialog<LibraryStatisticsView>(viewModel);
		}

		private static void ShowView<TView>(object dataContext)
			where TView : Window, new()
		{
			CreateView<TView>(dataContext).Show();
		}

		private static bool ShowDialog<TDialogView>(object dataContext)
			where TDialogView : Window, new()
		{
			return CreateView<TDialogView>(dataContext).ShowDialog() ?? false;
		}

		private static TView CreateView<TView>(object dataContext)
			where TView : Window, new()
		{
			return new TView
			{
				DataContext = dataContext,
				Owner = ApplicationWindow,
			};
		}
	}
}
