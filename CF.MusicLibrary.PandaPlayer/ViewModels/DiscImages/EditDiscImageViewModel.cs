using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Core.Facades;
using CF.MusicLibrary.Common.Images;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.PandaPlayer.Events.DiscEvents;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Options;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.DiscImages
{
	public class EditDiscImageViewModel : ViewModelBase, IEditDiscImageViewModel
	{
		private readonly IMusicLibrary musicLibrary;
		private readonly IDocumentDownloader documentDownloader;
		private readonly IImageFile imageFile;
		private readonly IFileSystemFacade fileSystemFacade;
		private readonly IWebBrowser webBrowser;
		private readonly PandaPlayerSettings settings;

		public Disc Disc { get; private set; }

		public string ImageFileName => imageFile.ImageFileName;

		public bool ImageIsValid => imageFile.ImageIsValid;

		private bool imageWasChanged;
		public bool ImageWasChanged
		{
			get { return imageWasChanged; }
			set { Set(ref imageWasChanged, value); }
		}

		public string ImageProperties => imageFile.ImageProperties;

		public string ImageStatus => imageFile.ImageStatus;

		public ICommand LaunchSearchForDiscImageCommand { get; }

		public EditDiscImageViewModel(IMusicLibrary musicLibrary, IDocumentDownloader documentDownloader, IImageFile imageFile,
			IFileSystemFacade fileSystemFacade, IWebBrowser webBrowser, IOptions<PandaPlayerSettings> options)
		{
			this.musicLibrary = musicLibrary ?? throw new ArgumentNullException(nameof(musicLibrary));
			this.documentDownloader = documentDownloader ?? throw new ArgumentNullException(nameof(documentDownloader));
			this.imageFile = imageFile ?? throw new ArgumentNullException(nameof(imageFile));
			this.fileSystemFacade = fileSystemFacade ?? throw new ArgumentNullException(nameof(fileSystemFacade));
			this.webBrowser = webBrowser ?? throw new ArgumentNullException(nameof(webBrowser));
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));

			imageFile.PropertyChanged += ImageFile_PropertyChanged;

			LaunchSearchForDiscImageCommand = new RelayCommand(LaunchSearchForDiscCoverImage);
		}

		public async Task Load(Disc disc)
		{
			Disc = disc;
			imageFile.Unload();

			var currDiscCoverImage = await musicLibrary.GetDiscCoverImage(disc);
			if (currDiscCoverImage != null)
			{
				imageFile.Load(currDiscCoverImage, false);
			}
			else
			{
				LaunchSearchForDiscCoverImage();
			}

			ImageWasChanged = false;
		}

		public void Unload()
		{
			Disc = null;
			imageFile.Unload();
		}

		public async Task Save()
		{
			if (Disc == null)
			{
				throw new InvalidOperationException("EditDiscImageViewModel is not loaded");
			}
			if (!ImageIsValid)
			{
				throw new InvalidOperationException("Current disc image is not valid");
			}
			if (!ImageWasChanged)
			{
				throw new InvalidOperationException("Image was not changed");
			}

			await musicLibrary.SetDiscCoverImage(Disc, imageFile.ImageInfo);
			Messenger.Default.Send(new DiscImageChangedEventArgs(Disc));
		}

		public async Task SetImage(Uri imageUri)
		{
			if (imageUri.IsFile)
			{
				string imageFileName = imageUri.LocalPath;
				if (fileSystemFacade.FileExists(imageFileName))
				{
					imageFile.Load(imageFileName, false);
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
			imageFile.Load(imageFileName, true);
			ImageWasChanged = true;
		}

		private void ImageFile_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			RaisePropertyChanged(e.PropertyName);
		}

		internal void LaunchSearchForDiscCoverImage()
		{
			foreach (var discCoverSearchPageStub in settings.DiscCoverImageLookupPages)
			{
				string discCoverSearchPage = discCoverSearchPageStub;
				discCoverSearchPage = discCoverSearchPage.Replace("{DiscArtist}", Uri.EscapeDataString(Disc.Artist?.Name ?? String.Empty));
				discCoverSearchPage = discCoverSearchPage.Replace("{DiscTitle}", Uri.EscapeDataString(Disc.AlbumTitle ?? String.Empty));
				webBrowser.OpenPage(discCoverSearchPage);
			}
		}
	}
}
