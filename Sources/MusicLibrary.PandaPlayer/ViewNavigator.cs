using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CF.Library.Core.Enums;
using CF.Library.Core.Interfaces;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using MusicLibrary.PandaPlayer.Views;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.PandaPlayer
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
				var message = Current($"You've just finished listening of playlist that have some songs unrated. Please devote some time and rate them.");
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

		public async Task ShowEditDiscImageView(DiscModel disc)
		{
			var viewModel = viewModelHolder.EditDiscImageViewModel;
			await viewModel.Load(disc);
			ShowDialog<EditDiscImageView>(viewModel);
		}

		public async Task ShowLibraryStatisticsView(CancellationToken cancellationToken)
		{
			var viewModel = viewModelHolder.LibraryStatisticsViewModel;
			await viewModel.Load(cancellationToken);
			ShowDialog<LibraryStatisticsView>(viewModel);
		}

		private static bool ShowDialog<TDialogView>(object dataContext)
			where TDialogView : Window, new()
		{
			TDialogView dialogView = new TDialogView
			{
				DataContext = dataContext,
				Owner = ApplicationWindow,
			};

			return dialogView.ShowDialog() ?? false;
		}
	}
}
