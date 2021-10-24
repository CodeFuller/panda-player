using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PandaPlayer.Core.Facades;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels.DiscImages
{
	public class DiscImageViewModel : ViewModelBase, IDiscImageViewModel
	{
		private readonly IViewNavigator viewNavigator;

		private readonly IFileSystemFacade fileSystemFacade;

		private DiscModel currentDisc;

		private DiscModel CurrentDisc
		{
			get => currentDisc;
			set
			{
				Set(ref currentDisc, value);
				RaisePropertyChanged(nameof(CoverImageSource));
			}
		}

		public DiscImageSource CoverImageSource
		{
			get
			{
				if (currentDisc == null)
				{
					return null;
				}

				if (currentDisc.IsDeleted)
				{
					return DiscImageSource.ForDeletedDisc;
				}

				if (currentDisc.CoverImage == null)
				{
					return null;
				}

				var imageContentUri = currentDisc.CoverImage.ContentUri;
				if (!fileSystemFacade.FileExists(imageContentUri.OriginalString))
				{
					return DiscImageSource.ForMissingImage;
				}

				return DiscImageSource.ForImage(imageContentUri);
			}
		}

		public ICommand EditDiscImageCommand { get; }

		public DiscImageViewModel(IViewNavigator viewNavigator, IFileSystemFacade fileSystemFacade)
		{
			this.viewNavigator = viewNavigator ?? throw new ArgumentNullException(nameof(viewNavigator));
			this.fileSystemFacade = fileSystemFacade ?? throw new ArgumentNullException(nameof(fileSystemFacade));

			EditDiscImageCommand = new RelayCommand(EditDiscImage);

			Messenger.Default.Register<ActiveDiscChangedEventArgs>(this, e => CurrentDisc = e.Disc);
			Messenger.Default.Register<DiscImageChangedEventArgs>(this, e => OnDiscImageChanged(e.Disc));
		}

		private void EditDiscImage()
		{
			var activeDisc = CurrentDisc;
			if (activeDisc == null)
			{
				return;
			}

			viewNavigator.ShowEditDiscImageView(activeDisc);
		}

		private void OnDiscImageChanged(DiscModel disc)
		{
			if (disc.Id == CurrentDisc?.Id)
			{
				RaisePropertyChanged(nameof(CoverImageSource));
			}
		}
	}
}
