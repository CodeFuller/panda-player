using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Wpf;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MusicLibrary.DiscAdder.ViewModels.Interfaces;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.DiscAdder.ViewModels
{
	internal class DiscAdderViewModel : ViewModelBase, IDiscAdderViewModel
	{
		private bool IsLoaded { get; set; }

		public ICommand SwitchToPrevPageCommand { get; }

		public ICommand SwitchToNextPageCommand { get; }

		private readonly IEditSourceContentViewModel editSourceContentViewModel;
		private readonly IEditDiscsDetailsViewModel editDiscsDetailsViewModel;
		private readonly IEditSourceDiscImagesViewModel editSourceDiscImagesViewModel;
		private readonly IEditSongsDetailsViewModel editSongsDetailsViewModel;
		private readonly IAddToLibraryViewModel addToLibraryViewModel;

		public DiscAdderViewModel(IEditSourceContentViewModel editSourceContentViewModel, IEditDiscsDetailsViewModel editDiscsDetailsViewModel,
			IEditSourceDiscImagesViewModel editSourceDiscImagesViewModel, IEditSongsDetailsViewModel editSongsDetailsViewModel, IAddToLibraryViewModel addToLibraryViewModel)
		{
			this.editSourceContentViewModel = editSourceContentViewModel ?? throw new ArgumentNullException(nameof(editSourceContentViewModel));
			this.editDiscsDetailsViewModel = editDiscsDetailsViewModel ?? throw new ArgumentNullException(nameof(editDiscsDetailsViewModel));
			this.editSourceDiscImagesViewModel = editSourceDiscImagesViewModel ?? throw new ArgumentNullException(nameof(editSourceDiscImagesViewModel));
			this.editSongsDetailsViewModel = editSongsDetailsViewModel ?? throw new ArgumentNullException(nameof(editSongsDetailsViewModel));
			this.addToLibraryViewModel = addToLibraryViewModel ?? throw new ArgumentNullException(nameof(addToLibraryViewModel));

			SwitchToPrevPageCommand = new RelayCommand(SwitchToPrevPage);
			SwitchToNextPageCommand = new AsyncRelayCommand(() => SwitchToNextPage(CancellationToken.None));

			currentPage = editSourceContentViewModel;

			foreach (var viewModel in ViewModels)
			{
				viewModel.PropertyChanged += ViewModel_PropertyChanged;
			}
		}

		private IEnumerable<IPageViewModel> ViewModels
		{
			get
			{
				yield return editSourceContentViewModel;
				yield return editDiscsDetailsViewModel;
				yield return editSourceDiscImagesViewModel;
				yield return editSongsDetailsViewModel;
				yield return addToLibraryViewModel;
			}
		}

		public bool CanSwitchToPrevPage => PrevPage != null;

		public bool CanSwitchToNextPage => NextPage != null && CurrentPage.DataIsReady;

		public string PrevPageName => PrevPage?.Name ?? "N/A";

		public string NextPageName
		{
			get
			{
				if (NextPage == editSourceContentViewModel)
				{
					return "Add More Discs";
				}

				return NextPage?.Name ?? "N/A";
			}
		}

		private IPageViewModel currentPage;

		public IPageViewModel CurrentPage
		{
			get => currentPage;
			set
			{
				Set(ref currentPage, value);

				RaisePropertyChanged(nameof(CanSwitchToPrevPage));
				RaisePropertyChanged(nameof(CanSwitchToNextPage));

				RaisePropertyChanged(nameof(PrevPageName));
				RaisePropertyChanged(nameof(NextPageName));
			}
		}

		protected IPageViewModel PrevPage
		{
			get
			{
				if (CurrentPage == editDiscsDetailsViewModel)
				{
					return editSourceContentViewModel;
				}

				if (CurrentPage == editSourceDiscImagesViewModel)
				{
					return editDiscsDetailsViewModel;
				}

				if (CurrentPage == editSongsDetailsViewModel)
				{
					return editSourceDiscImagesViewModel;
				}

				return null;
			}
		}

		protected IPageViewModel NextPage
		{
			get
			{
				if (CurrentPage == editSourceContentViewModel)
				{
					return editDiscsDetailsViewModel;
				}

				if (CurrentPage == editDiscsDetailsViewModel)
				{
					return editSourceDiscImagesViewModel;
				}

				if (CurrentPage == editSourceDiscImagesViewModel)
				{
					return editSongsDetailsViewModel;
				}

				if (CurrentPage == editSongsDetailsViewModel)
				{
					return addToLibraryViewModel;
				}

				if (CurrentPage == addToLibraryViewModel)
				{
					return editSourceContentViewModel;
				}

				return null;
			}
		}

		public async Task Load(CancellationToken cancellationToken)
		{
			if (IsLoaded)
			{
				return;
			}

			await editSourceContentViewModel.LoadDefaultContent(cancellationToken);

			IsLoaded = true;
		}

		private async Task SwitchToNextPage(CancellationToken cancellationToken)
		{
			var nextPage = NextPage;

			if (nextPage == null)
			{
				throw new InvalidOperationException(Current($"Could not switch forward from the page {CurrentPage.Name}"));
			}

			if (nextPage == editDiscsDetailsViewModel)
			{
				await editDiscsDetailsViewModel.SetDiscs(editSourceContentViewModel.AddedDiscs, cancellationToken);
			}
			else if (nextPage == editSourceDiscImagesViewModel)
			{
				editSourceDiscImagesViewModel.LoadImages(editDiscsDetailsViewModel.AddedDiscs);
			}
			else if (nextPage == editSongsDetailsViewModel)
			{
				editSongsDetailsViewModel.SetSongs(editDiscsDetailsViewModel.AddedSongs);
			}
			else if (nextPage == addToLibraryViewModel)
			{
				var addedSongs = editSongsDetailsViewModel.Songs.Select(s => s.AddedSong).ToList();
				addToLibraryViewModel.SetSongs(addedSongs);
				addToLibraryViewModel.SetDiscsImages(editSourceDiscImagesViewModel.AddedImages);
			}
			else if (nextPage == editSourceContentViewModel)
			{
				await Load(cancellationToken);
			}

			CurrentPage = nextPage;
		}

		private void SwitchToPrevPage()
		{
			CurrentPage = PrevPage ?? throw new InvalidOperationException(Current($"Could not switch back from the page {CurrentPage.Name}"));
		}

		private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(IPageViewModel.DataIsReady))
			{
				RaisePropertyChanged(nameof(CanSwitchToNextPage));
			}
		}
	}
}
