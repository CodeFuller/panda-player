using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Events.DiscEvents;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace MusicLibrary.PandaPlayer.ViewModels.DiscImages
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

		public DiscImageViewModel(IViewNavigator viewNavigator)
		{
			this.viewNavigator = viewNavigator ?? throw new ArgumentNullException(nameof(viewNavigator));

			Messenger.Default.Register<ActiveDiscChangedEventArgs>(this, e => CurrentDisc = e.Disc);
			Messenger.Default.Register<DiscImageChangedEventArgs>(this, e => OnDiscImageChanged(e.Disc));
		}

		public async Task EditDiscImage()
		{
			var activeDisc = CurrentDisc;
			if (activeDisc == null)
			{
				return;
			}

			await viewNavigator.ShowEditDiscImageView(activeDisc);
		}

		private void OnDiscImageChanged(DiscModel disc)
		{
			// TODO: Restore this functionality
			if (disc == CurrentDisc)
			{
				// CurrentImageFileName = GetCurrentImageFileName();
			}
		}
	}
}
