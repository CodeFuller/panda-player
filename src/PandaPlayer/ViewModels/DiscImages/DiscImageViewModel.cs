using System;
using System.IO;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels.DiscImages
{
	public class DiscImageViewModel : ViewModelBase, IDiscImageViewModel
	{
		private readonly IViewNavigator viewNavigator;

		private DiscModel currentDisc;

		private DiscModel CurrentDisc
		{
			get => currentDisc;
			set
			{
				Set(ref currentDisc, value);
				RaisePropertyChanged(nameof(CurrentImageUri));
			}
		}

		public Uri CurrentImageUri
		{
			get
			{
				if (currentDisc == null)
				{
					return null;
				}

				if (currentDisc.IsDeleted)
				{
					return new Uri("pack://application:,,,/PandaPlayer;component/Views/Icons/Deleted.png", UriKind.Absolute);
				}

				if (currentDisc.CoverImage == null)
				{
					return null;
				}

				var imageContentUri = currentDisc.CoverImage.ContentUri;
				if (!File.Exists(imageContentUri.OriginalString))
				{
					return new Uri("pack://application:,,,/PandaPlayer;component/Views/Icons/ImageNotFound.png", UriKind.Absolute);
				}

				return imageContentUri;
			}
		}

		public ICommand EditDiscImageCommand { get; }

		public DiscImageViewModel(IViewNavigator viewNavigator)
		{
			this.viewNavigator = viewNavigator ?? throw new ArgumentNullException(nameof(viewNavigator));

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
				RaisePropertyChanged(nameof(CurrentImageUri));
			}
		}
	}
}
