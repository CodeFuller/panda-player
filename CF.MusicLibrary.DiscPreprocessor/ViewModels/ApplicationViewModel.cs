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
		private readonly IEditDiscsDetailsViewModel editDiscsesDetailsViewModel;
		private readonly IEditSongsDetailsViewModel editSongsDetailsViewModel;
		private readonly IAddToLibraryViewModel addToLibraryViewModel;

		public ApplicationViewModel(IEditSourceContentViewModel editSourceContentViewModel, IEditDiscsDetailsViewModel editDiscsesDetailsViewModel,
			IEditSongsDetailsViewModel editSongsDetailsViewModel, IAddToLibraryViewModel addToLibraryViewModel)
		{
			if (editSourceContentViewModel == null)
			{
				throw new ArgumentNullException(nameof(editSourceContentViewModel));
			}
			if (editDiscsesDetailsViewModel == null)
			{
				throw new ArgumentNullException(nameof(editDiscsesDetailsViewModel));
			}
			if (editSongsDetailsViewModel == null)
			{
				throw new ArgumentNullException(nameof(editSongsDetailsViewModel));
			}
			if (addToLibraryViewModel == null)
			{
				throw new ArgumentNullException(nameof(addToLibraryViewModel));
			}

			this.editSourceContentViewModel = editSourceContentViewModel;
			this.editDiscsesDetailsViewModel = editDiscsesDetailsViewModel;
			this.editSongsDetailsViewModel = editSongsDetailsViewModel;
			this.addToLibraryViewModel = addToLibraryViewModel;

			SwitchToPrevPageCommand = new RelayCommand(SwitchToPrevPage);
			SwitchToNextPageCommand = new AsyncRelayCommand(SwitchToNextPage);

			currPage = editSourceContentViewModel;

			foreach (var viewModel in ViewModels)
			{
				viewModel.PropertyChanged += ViewModel_PropertyChanged;
			}
		}

		IEnumerable<IPageViewModel> ViewModels
		{
			get
			{
				yield return editSourceContentViewModel;
				yield return editDiscsesDetailsViewModel;
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
			get { return currPage; }
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
				if (CurrPage == editDiscsesDetailsViewModel)
				{
					return editSourceContentViewModel;
				}

				if (CurrPage == editSongsDetailsViewModel)
				{
					return editDiscsesDetailsViewModel;
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
					return editDiscsesDetailsViewModel;
				}

				if (CurrPage == editDiscsesDetailsViewModel)
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

		private async Task SwitchToNextPage()
		{
			var nextPage = NextPage;

			if (nextPage == null)
			{
				throw new InvalidOperationException(Current($"Could not switch forward from the page {CurrPage.Name}"));
			}

			if (nextPage == editDiscsesDetailsViewModel)
			{
				await editDiscsesDetailsViewModel.SetDiscs(editSourceContentViewModel.AddedDiscs);
			}
			else if (nextPage == editSongsDetailsViewModel)
			{
				editSongsDetailsViewModel.SetSongs(editDiscsesDetailsViewModel.AddedSongs);
			}
			else if (nextPage == addToLibraryViewModel)
			{
				addToLibraryViewModel.SetSongs(editSongsDetailsViewModel.Songs.Select(s => s.AddedSong));
				addToLibraryViewModel.SetDiscsCoverImages(editDiscsesDetailsViewModel.DiscCoverImages);
			}
			else if (nextPage == editSourceContentViewModel)
			{
				Load();
			}

			CurrPage = nextPage;
		}

		private void SwitchToPrevPage()
		{
			if (PrevPage == null)
			{
				throw new InvalidOperationException(Current($"Could not switch back from the page {CurrPage.Name}"));
			}

			CurrPage = PrevPage;
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
