using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Core.Facades;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Options;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Events.DiscEvents;
using MusicLibrary.PandaPlayer.Settings;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using MusicLibrary.Services.Interfaces;
using MusicLibrary.Shared.Extensions;
using MusicLibrary.Shared.Images;

namespace MusicLibrary.PandaPlayer.ViewModels.DiscImages
{
	public class EditDiscImageViewModel : ViewModelBase, IEditDiscImageViewModel
	{
		private readonly IDiscsService discsService;
		private readonly IDocumentDownloader documentDownloader;
		private readonly IImageFile imageFile;
		private readonly IFileSystemFacade fileSystemFacade;
		private readonly IWebBrowser webBrowser;
		private readonly PandaPlayerSettings settings;

		public DiscModel Disc { get; private set; }

		public string ImageFileName => imageFile.ImageFileName;

		public bool ImageIsValid => imageFile.ImageIsValid;

		private bool imageWasChanged;

		public bool ImageWasChanged
		{
			get => imageWasChanged;
			set => Set(ref imageWasChanged, value);
		}

		public string ImageProperties => imageFile.ImageProperties;

		public string ImageStatus => imageFile.ImageStatus;

		public ICommand LaunchSearchForDiscImageCommand { get; }

		public EditDiscImageViewModel(IDiscsService discsService, IDocumentDownloader documentDownloader, IImageFile imageFile,
			IFileSystemFacade fileSystemFacade, IWebBrowser webBrowser, IOptions<PandaPlayerSettings> options)
		{
			this.discsService = discsService ?? throw new ArgumentNullException(nameof(discsService));
			this.documentDownloader = documentDownloader ?? throw new ArgumentNullException(nameof(documentDownloader));
			this.imageFile = imageFile ?? throw new ArgumentNullException(nameof(imageFile));
			this.fileSystemFacade = fileSystemFacade ?? throw new ArgumentNullException(nameof(fileSystemFacade));
			this.webBrowser = webBrowser ?? throw new ArgumentNullException(nameof(webBrowser));
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));

			imageFile.PropertyChanged += ImageFile_PropertyChanged;

			LaunchSearchForDiscImageCommand = new RelayCommand(LaunchSearchForDiscCoverImage);
		}

		public void Load(DiscModel disc)
		{
			Disc = disc;
			imageFile.Unload();

			var currentImageFileName = disc.CoverImage?.ContentUri.OriginalString;
			if (currentImageFileName != null)
			{
				imageFile.Load(currentImageFileName, false);
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

		public async Task Save(CancellationToken cancellationToken)
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

			var coverImage = new DiscImageModel
			{
				Disc = Disc,
				TreeTitle = imageFile.ImageInfo.GetDiscCoverImageTreeTitle(),
				ImageType = DiscImageType.Cover,
			};

			using var imageContent = File.OpenRead(imageFile.ImageFileName);
			await discsService.SetDiscCoverImage(coverImage, imageContent, cancellationToken);

			Messenger.Default.Send(new DiscImageChangedEventArgs(Disc));
		}

		public async Task SetImage(Uri imageUri)
		{
			if (imageUri.IsFile)
			{
				var imageFileName = imageUri.LocalPath;
				if (fileSystemFacade.FileExists(imageFileName))
				{
					imageFile.Load(imageFileName, false);
					ImageWasChanged = true;
				}

				return;
			}

			var imageContent = await documentDownloader.Download(imageUri);
			SetImage(imageContent);
		}

		public void SetImage(byte[] imageData)
		{
			var imageFileName = fileSystemFacade.GetTempFileName();
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
			var artistName = Disc.SoloArtist?.Name ?? String.Empty;
			var albumTitle = Disc.AlbumTitle ?? String.Empty;

			foreach (var discCoverSearchPageStub in settings.DiscCoverImageLookupPages)
			{
				var discCoverSearchPage = discCoverSearchPageStub;
				discCoverSearchPage = discCoverSearchPage.Replace("{DiscArtist}", Uri.EscapeDataString(artistName), StringComparison.Ordinal);
				discCoverSearchPage = discCoverSearchPage.Replace("{DiscTitle}", Uri.EscapeDataString(albumTitle), StringComparison.Ordinal);
				webBrowser.OpenPage(discCoverSearchPage);
			}
		}
	}
}
