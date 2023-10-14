using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PandaPlayer.Core.Facades;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels.DiscImages
{
	public class DiscImageViewModel : ObservableObject, IDiscImageViewModel
	{
		private readonly IViewNavigator viewNavigator;

		private readonly IFileSystemFacade fileSystemFacade;

		private DiscModel currentDisc;

		private DiscModel CurrentDisc
		{
			get => currentDisc;
			set
			{
				SetProperty(ref currentDisc, value);
				OnPropertyChanged(nameof(CoverImageSource));
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

		public DiscImageViewModel(IViewNavigator viewNavigator, IFileSystemFacade fileSystemFacade, IMessenger messenger)
		{
			this.viewNavigator = viewNavigator ?? throw new ArgumentNullException(nameof(viewNavigator));
			this.fileSystemFacade = fileSystemFacade ?? throw new ArgumentNullException(nameof(fileSystemFacade));

			EditDiscImageCommand = new RelayCommand(EditDiscImage);

			_ = messenger ?? throw new ArgumentNullException(nameof(messenger));
			messenger.Register<ActiveDiscChangedEventArgs>(this, (_, e) => CurrentDisc = e.Disc);
			messenger.Register<DiscImageChangedEventArgs>(this, (_, e) => OnDiscImageChanged(e.Disc));
		}

		private void EditDiscImage()
		{
			var activeDisc = CurrentDisc;
			if (activeDisc == null || activeDisc.IsDeleted)
			{
				return;
			}

			viewNavigator.ShowEditDiscImageView(activeDisc);
		}

		private void OnDiscImageChanged(DiscModel disc)
		{
			if (disc.Id == CurrentDisc?.Id)
			{
				OnPropertyChanged(nameof(CoverImageSource));
			}
		}
	}
}
