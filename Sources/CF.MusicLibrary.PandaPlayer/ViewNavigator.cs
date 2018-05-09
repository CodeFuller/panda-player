using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CF.Library.Core.Enums;
using CF.Library.Core.Interfaces;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using CF.MusicLibrary.PandaPlayer.Views;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.PandaPlayer
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

		public void ShowRateDiscView(Disc disc)
		{
			var unratedSongsNumber = disc.Songs.Count(s => s.Rating == null);
			if (unratedSongsNumber == 0)
			{
				throw new InvalidOperationException("All disc songs are already rated");
			}

			if (unratedSongsNumber == disc.Songs.Count())
			{
				var rateDiscViewModel = viewModelHolder.RateDiscViewModel;
				rateDiscViewModel.Load(disc);
				ShowDialog<RateDiscView>(rateDiscViewModel);
			}
			else
			{
				windowService.ShowMessageBox(Current($"You've just finished listening of disc '{disc.Title}' that have some songs still unrated. Please devote some time and rate them."),
					"Rate listened disc", ShowMessageBoxButton.Ok, ShowMessageBoxIcon.Exclamation);
			}
		}

		public void ShowDiscPropertiesView(Disc disc)
		{
			var viewModel = viewModelHolder.EditDiscPropertiesViewModel;
			viewModel.Load(disc);
			ShowDialog<EditDiscPropertiesView>(viewModel);
		}

		public void ShowSongPropertiesView(IEnumerable<Song> songs)
		{
			var viewModel = viewModelHolder.EditSongPropertiesViewModel;
			viewModel.Load(songs);
			ShowDialog<EditSongPropertiesView>(viewModel);
		}

		public async Task ShowEditDiscImageView(Disc disc)
		{
			var viewModel = viewModelHolder.EditDiscImageViewModel;
			await viewModel.Load(disc);
			ShowDialog<EditDiscImageView>(viewModel);
		}

		public void ShowLibraryStatisticsView()
		{
			ShowDialog<LibraryStatisticsView>(viewModelHolder.LibraryStatisticsViewModel);
		}

		private static bool ShowDialog<TDialogView>(object dataContext) where TDialogView : Window, new()
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
