using System;
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
				CurrentImageUri = currentDisc?.CoverImage?.ContentUri;
			}
		}

		private Uri currentImageUri;

		public Uri CurrentImageUri
		{
			get => currentImageUri;
			private set
			{
				// Why don't we use ViewModelBase.Set(ref currentImageUri, value)?
				// When disc image is updated with new file, CurrentImageUri is not actually changed, however
				// we need PropertyChanged event to be fired so that Image control updated image in the view.
				// Seems like ViewModelBase.Set() has some internal check whether new value equals to the old one
				// and don't fire the event in this case. That's why we should raise event manually.
				currentImageUri = value;
				RaisePropertyChanged();
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
				CurrentImageUri = disc.CoverImage?.ContentUri;
			}
		}
	}
}
