using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Wpf;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels
{
	public class ApplicationViewModel : ViewModelBase
	{
		public ICommand SwitchToPrevPageCommand { get; }

		public ICommand SwitchToNextPageCommand { get; }

		private readonly IEditSourceContentViewModel editSourceContentViewModel;
		private readonly IEditAlbumsDetailsViewModel editAlbumsDetailsViewModel;
		private readonly IEditSongsDetailsViewModel editSongsDetailsViewModel;
		private readonly IAddToLibraryViewModel addToLibraryViewModel;

		public ApplicationViewModel(IEditSourceContentViewModel editSourceContentViewModel, IEditAlbumsDetailsViewModel editAlbumsDetailsViewModel,
			IEditSongsDetailsViewModel editSongsDetailsViewModel, IAddToLibraryViewModel addToLibraryViewModel)
		{
			if (editSourceContentViewModel == null)
			{
				throw new ArgumentNullException(nameof(editSourceContentViewModel));
			}
			if (editAlbumsDetailsViewModel == null)
			{
				throw new ArgumentNullException(nameof(editAlbumsDetailsViewModel));
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
			this.editAlbumsDetailsViewModel = editAlbumsDetailsViewModel;
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
				yield return editAlbumsDetailsViewModel;
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
					return "Add More Albums";
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
				if (CurrPage == editAlbumsDetailsViewModel)
				{
					return editSourceContentViewModel;
				}

				if (CurrPage == editSongsDetailsViewModel)
				{
					return editAlbumsDetailsViewModel;
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
					return editAlbumsDetailsViewModel;
				}

				if (CurrPage == editAlbumsDetailsViewModel)
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

			if (nextPage == editAlbumsDetailsViewModel)
			{
				await editAlbumsDetailsViewModel.SetAlbums(editSourceContentViewModel.CurrentAlbums);
			}
			else if (nextPage == editSongsDetailsViewModel)
			{
				editSongsDetailsViewModel.SetSongs(editAlbumsDetailsViewModel.Songs);
			}
			else if (nextPage == addToLibraryViewModel)
			{
				addToLibraryViewModel.SetSongsTagData(editSongsDetailsViewModel.Songs.Select(s => s.TagData));
				addToLibraryViewModel.SetAlbumCoverImages(editAlbumsDetailsViewModel.AlbumCoverImages);
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
