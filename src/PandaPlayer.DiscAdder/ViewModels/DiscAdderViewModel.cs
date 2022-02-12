using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CodeFuller.Library.Wpf;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PandaPlayer.DiscAdder.ViewModels.Interfaces;

namespace PandaPlayer.DiscAdder.ViewModels
{
	internal class DiscAdderViewModel : ViewModelBase, IDiscAdderViewModel
	{
		private class NextPageInfo
		{
			private readonly string title;

			public string Title
			{
				get => title ?? NextPage.Name;
				init => title = value;
			}

			public IPageViewModel NextPage { get; init; }

			public Func<CancellationToken, Task> NextPageAction { get; init; }
		}

		private readonly IEditSourceContentViewModel editSourceContentViewModel;
		private readonly IEditDiscsDetailsViewModel editDiscsDetailsViewModel;
		private readonly IEditSourceDiscImagesViewModel editSourceDiscImagesViewModel;
		private readonly IEditSongsDetailsViewModel editSongsDetailsViewModel;
		private readonly IAddToLibraryViewModel addToLibraryViewModel;

		private bool IsLoaded { get; set; }

		public ICommand SwitchToPrevPageCommand { get; }

		public ICommand SwitchToNextPageCommand { get; }

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

		public event EventHandler OnRequestClose;

		public bool CanSwitchToPrevPage => PrevPage != null && (CurrentPage is not IAddToLibraryViewModel || addToLibraryViewModel.CanAddContent);

		public bool CanSwitchToNextPage => CurrentPage.DataIsReady;

		public string PrevPageName => PrevPage?.Name ?? "N/A";

		public string NextPageName => NextPage.Title;

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

		private IPageViewModel PrevPage
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

				if (CurrentPage == addToLibraryViewModel)
				{
					return editSongsDetailsViewModel;
				}

				return null;
			}
		}

		private NextPageInfo NextPage
		{
			get
			{
				if (CurrentPage == editSourceContentViewModel)
				{
					return new NextPageInfo
					{
						NextPage = editDiscsDetailsViewModel,
						NextPageAction = cancellationToken => editDiscsDetailsViewModel.SetDiscs(editSourceContentViewModel.AddedDiscs, cancellationToken),
					};
				}

				if (CurrentPage == editDiscsDetailsViewModel)
				{
					return new NextPageInfo
					{
						NextPage = editSourceDiscImagesViewModel,
						NextPageAction = _ =>
						{
							editSourceDiscImagesViewModel.Load(editDiscsDetailsViewModel.Discs);
							return Task.CompletedTask;
						},
					};
				}

				if (CurrentPage == editSourceDiscImagesViewModel)
				{
					return new NextPageInfo
					{
						NextPage = editSongsDetailsViewModel,
						NextPageAction = _ =>
						{
							editSongsDetailsViewModel.Load(editDiscsDetailsViewModel.Discs);
							return Task.CompletedTask;
						},
					};
				}

				if (CurrentPage == editSongsDetailsViewModel)
				{
					return new NextPageInfo
					{
						NextPage = addToLibraryViewModel,
						NextPageAction = _ =>
						{
							addToLibraryViewModel.Load(editSongsDetailsViewModel.Songs, editSourceDiscImagesViewModel.ImageItems);
							return Task.CompletedTask;
						},
					};
				}

				if (CurrentPage == addToLibraryViewModel)
				{
					return new NextPageInfo
					{
						Title = "Close",
						NextPage = editSourceContentViewModel,
						NextPageAction = async cancellationToken =>
						{
							await editSourceContentViewModel.ResetContent(cancellationToken);

							OnRequestClose?.Invoke(this, EventArgs.Empty);

							IsLoaded = false;
						},
					};
				}

				throw new InvalidOperationException("The type of current page is unknown");
			}
		}

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

		public async Task Load(CancellationToken cancellationToken)
		{
			if (IsLoaded)
			{
				return;
			}

			await editSourceContentViewModel.Load(cancellationToken);

			IsLoaded = true;
		}

		private async Task SwitchToNextPage(CancellationToken cancellationToken)
		{
			var nextPage = NextPage;

			await nextPage.NextPageAction(cancellationToken);

			CurrentPage = nextPage.NextPage;
		}

		private void SwitchToPrevPage()
		{
			CurrentPage = PrevPage ?? throw new InvalidOperationException($"Could not switch back from the page {CurrentPage.Name}");
		}

		private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(IPageViewModel.DataIsReady))
			{
				RaisePropertyChanged(nameof(CanSwitchToNextPage));
			}

			if (sender is IAddToLibraryViewModel && e.PropertyName == nameof(IAddToLibraryViewModel.CanAddContent))
			{
				RaisePropertyChanged(nameof(CanSwitchToPrevPage));
			}
		}
	}
}
