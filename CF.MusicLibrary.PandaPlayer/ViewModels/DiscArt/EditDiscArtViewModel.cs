using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Core.Configuration;
using CF.Library.Core.Facades;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.Common.DiscArt;
using CF.MusicLibrary.PandaPlayer.Events.DiscEvents;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.DiscArt
{
	public class EditDiscArtViewModel : ViewModelBase, IEditDiscArtViewModel
	{
		private readonly IMusicLibrary musicLibrary;
		private readonly IDocumentDownloader documentDownloader;
		private readonly IFileSystemFacade fileSystemFacade;
		private readonly IWebBrowser webBrowser;

		private IDiscArtImageFile discArtImageFile;
		/// <summary>
		/// Property injection for IDiscArtImageFile.
		/// </summary>
		internal IDiscArtImageFile DiscArtImageFile
		{
			get { return discArtImageFile; }
			set
			{
				if (discArtImageFile != null)
				{
					discArtImageFile.PropertyChanged -= DiscArtImageFileOnPropertyChanged;
				}

				discArtImageFile = value;

				if (discArtImageFile != null)
				{
					discArtImageFile.PropertyChanged += DiscArtImageFileOnPropertyChanged;
				}
			}
		}

		public Disc Disc { get; private set; }

		public string ImageFileName => DiscArtImageFile?.ImageFileName;

		public bool ImageIsValid => DiscArtImageFile.ImageIsValid;

		private bool imageWasChanged;
		public bool ImageWasChanged
		{
			get { return imageWasChanged; }
			set { Set(ref imageWasChanged, value); }
		}

		public string ImageProperties => DiscArtImageFile.ImageProperties;

		public string ImageStatus => DiscArtImageFile.ImageStatus;

		public ICommand LaunchSearchForDiscArtCommand { get; }

		public EditDiscArtViewModel(IMusicLibrary musicLibrary, IDocumentDownloader documentDownloader, IDiscArtValidator discArtValidator,
			IFileSystemFacade fileSystemFacade, IWebBrowser webBrowser)
		{
			if (musicLibrary == null)
			{
				throw new ArgumentNullException(nameof(musicLibrary));
			}
			if (documentDownloader == null)
			{
				throw new ArgumentNullException(nameof(documentDownloader));
			}
			if (discArtValidator == null)
			{
				throw new ArgumentNullException(nameof(discArtValidator));
			}
			if (fileSystemFacade == null)
			{
				throw new ArgumentNullException(nameof(fileSystemFacade));
			}
			if (webBrowser == null)
			{
				throw new ArgumentNullException(nameof(webBrowser));
			}

			this.musicLibrary = musicLibrary;
			this.documentDownloader = documentDownloader;
			this.fileSystemFacade = fileSystemFacade;
			this.webBrowser = webBrowser;

			DiscArtImageFile = new DiscArtImageFile(discArtValidator, fileSystemFacade);

			LaunchSearchForDiscArtCommand = new RelayCommand(LaunchSearchForDiscArt);
		}

		public async Task Load(Disc disc)
		{
			Disc = disc;
			DiscArtImageFile.Unload();

			var currDiscCoverImage = await musicLibrary.GetDiscCoverImage(disc);
			if (currDiscCoverImage != null)
			{
				DiscArtImageFile.Load(currDiscCoverImage, false);
			}
			else
			{
				LaunchSearchForDiscArt();
			}

			ImageWasChanged = false;
		}

		public void Unload()
		{
			Disc = null;
			DiscArtImageFile.Unload();
		}

		public async Task Save()
		{
			if (Disc == null)
			{
				throw new InvalidOperationException("EditDiscArtViewModel is not loaded");
			}
			if (!ImageIsValid)
			{
				throw new InvalidOperationException("Current disc image is not valid");
			}
			if (!ImageWasChanged)
			{
				throw new InvalidOperationException("Image was not changed");
			}

			await musicLibrary.SetDiscCoverImage(Disc, ImageFileName);
			Messenger.Default.Send(new DiscArtChangedEventArgs(Disc));
		}

		public async Task SetImage(Uri imageUri)
		{
			if (imageUri.IsFile)
			{
				string imageFileName = imageUri.LocalPath;
				if (fileSystemFacade.FileExists(imageFileName))
				{
					DiscArtImageFile.Load(imageFileName, false);
					ImageWasChanged = true;
				}
				return;
			}

			byte[] imageContent = await documentDownloader.Download(imageUri);
			SetImage(imageContent);
		}

		public void SetImage(byte[] imageData)
		{
			string imageFileName = fileSystemFacade.GetTempFileName();
			fileSystemFacade.WriteAllBytes(imageFileName, imageData);
			DiscArtImageFile.Load(imageFileName, true);
			ImageWasChanged = true;
		}

		private void DiscArtImageFileOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			RaisePropertyChanged(e.PropertyName);
		}

		internal void LaunchSearchForDiscArt()
		{
			foreach (var discCoverSearchPageStub in AppSettings.GetOptionalValues<string>("DiscCoverImageLookupPages"))
			{
				string discCoverSearchPage = discCoverSearchPageStub;
				discCoverSearchPage = discCoverSearchPage.Replace("{DiscArtist}", Disc.Artist?.Name ?? String.Empty);
				discCoverSearchPage = discCoverSearchPage.Replace("{DiscTitle}", Disc.AlbumTitle ?? String.Empty);
				webBrowser.OpenPage(new Uri(discCoverSearchPage));
			}
		}
	}
}
