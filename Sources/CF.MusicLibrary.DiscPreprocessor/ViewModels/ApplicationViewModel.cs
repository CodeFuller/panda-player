using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Wpf;
using CF.MusicLibrary.DiscPreprocessor.ViewModels.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.DiscPreprocessor.ViewModels
{
	public class ApplicationViewModel : ViewModelBase
	{
		public ICommand SwitchToPrevPageCommand { get; }

		public ICommand SwitchToNextPageCommand { get; }

		private readonly IEditSourceContentViewModel editSourceContentViewModel;
		private readonly IEditDiscsDetailsViewModel editDiscsDetailsViewModel;
		private readonly IEditSourceDiscImagesViewModel editSourceDiscImagesViewModel;
		private readonly IEditSongsDetailsViewModel editSongsDetailsViewModel;
		private readonly IAddToLibraryViewModel addToLibraryViewModel;

		public ApplicationViewModel(IEditSourceContentViewModel editSourceContentViewModel, IEditDiscsDetailsViewModel editDiscsDetailsViewModel,
			IEditSourceDiscImagesViewModel editSourceDiscImagesViewModel, IEditSongsDetailsViewModel editSongsDetailsViewModel, IAddToLibraryViewModel addToLibraryViewModel)
		{
			this.editSourceContentViewModel = editSourceContentViewModel ?? throw new ArgumentNullException(nameof(editSourceContentViewModel));
			this.editDiscsDetailsViewModel = editDiscsDetailsViewModel ?? throw new ArgumentNullException(nameof(editDiscsDetailsViewModel));
			this.editSourceDiscImagesViewModel = editSourceDiscImagesViewModel ?? throw new ArgumentNullException(nameof(editSourceDiscImagesViewModel));
			this.editSongsDetailsViewModel = editSongsDetailsViewModel ?? throw new ArgumentNullException(nameof(editSongsDetailsViewModel));
			this.addToLibraryViewModel = addToLibraryViewModel ?? throw new ArgumentNullException(nameof(addToLibraryViewModel));

			SwitchToPrevPageCommand = new RelayCommand(SwitchToPrevPage);
			SwitchToNextPageCommand = new AsyncRelayCommand(SwitchToNextPage);

			currPage = editSourceContentViewModel;

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

		public bool CanSwitchToNextPage => NextPage != null && CurrPage.DataIsReady;

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

		private IPageViewModel currPage;

		public IPageViewModel CurrPage
		{
			get => currPage;
			set
			{
				Set(ref currPage, value);

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
				if (CurrPage == editDiscsDetailsViewModel)
				{
					return editSourceContentViewModel;
				}

				if (CurrPage == editSourceDiscImagesViewModel)
				{
					return editDiscsDetailsViewModel;
				}

				if (CurrPage == editSongsDetailsViewModel)
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
				if (CurrPage == editSourceContentViewModel)
				{
					return editDiscsDetailsViewModel;
				}

				if (CurrPage == editDiscsDetailsViewModel)
				{
					return editSourceDiscImagesViewModel;
				}

				if (CurrPage == editSourceDiscImagesViewModel)
				{
					return editSongsDetailsViewModel;
				}

				if (CurrPage == editSongsDetailsViewModel)
				{
					return addToLibraryViewModel;
				}

				if (CurrPage == addToLibraryViewModel)
				{
					return editSourceContentViewModel;
				}

				return null;
			}
		}

		public void Load()
		{
			editSourceContentViewModel.LoadDefaultContent();
		}

		internal async Task SwitchToNextPage()
		{
			var nextPage = NextPage;

			if (nextPage == null)
			{
				throw new InvalidOperationException(Current($"Could not switch forward from the page {CurrPage.Name}"));
			}

			if (nextPage == editDiscsDetailsViewModel)
			{
				await editDiscsDetailsViewModel.SetDiscs(editSourceContentViewModel.AddedDiscs);
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
				Load();
			}

			CurrPage = nextPage;
		}

		private void SwitchToPrevPage()
		{
			CurrPage = PrevPage ?? throw new InvalidOperationException(Current($"Could not switch back from the page {CurrPage.Name}"));
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
