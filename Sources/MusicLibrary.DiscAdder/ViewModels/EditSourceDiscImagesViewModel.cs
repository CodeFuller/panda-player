using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CF.Library.Core.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MusicLibrary.Core.Models;
using MusicLibrary.DiscAdder.AddingToLibrary;
using MusicLibrary.DiscAdder.ViewModels.Interfaces;
using MusicLibrary.Shared.Images;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.DiscAdder.ViewModels
{
	public class EditSourceDiscImagesViewModel : ViewModelBase, IEditSourceDiscImagesViewModel
	{
		private readonly IContentCrawler contentCrawler;
		private readonly IObjectFactory<IImageFile> imageFileFactory;

		private List<AddedDisc> addedDiscs;

		public string Name => "Edit Disc Images";

		public bool DataIsReady => ImageItems.All(im => im.ImageIsValid);

		public ObservableCollection<DiscImageViewItem> ImageItems { get; }

		public IEnumerable<AddedDiscImage> AddedImages => ImageItems.Select(im => new AddedDiscImage(im.Disc, im.ImageInfo));

		public ICommand RefreshContentCommand { get; }

		public EditSourceDiscImagesViewModel(IContentCrawler contentCrawler, IObjectFactory<IImageFile> imageFileFactory)
		{
			this.contentCrawler = contentCrawler ?? throw new ArgumentNullException(nameof(contentCrawler));
			this.imageFileFactory = imageFileFactory ?? throw new ArgumentNullException(nameof(imageFileFactory));

			ImageItems = new ObservableCollection<DiscImageViewItem>();

			RefreshContentCommand = new RelayCommand(RefreshContent);
		}

		public void LoadImages(IEnumerable<AddedDisc> discs)
		{
			addedDiscs = discs.Where(d => d.IsNewDisc).ToList();
			LoadImages();
		}

		internal void RefreshContent()
		{
			LoadImages();
		}

		private void LoadImages()
		{
			ImageItems.Clear();

			foreach (var discInfo in addedDiscs)
			{
				var discImages = contentCrawler.LoadDiscImages(discInfo.SourcePath).ToList();
				if (discImages.Count > 1)
				{
					throw new InvalidOperationException(Current($"Disc '{discInfo.SourcePath}' contains multiple images. Only one image per disc (cover image) is currently supported."));
				}

				var imageFile = imageFileFactory.CreateInstance();
				if (discImages.Count == 1)
				{
					imageFile.Load(discImages.Single(), false);
				}

				ImageItems.Add(new DiscImageViewItem(discInfo.Disc, DiscImageType.Cover, imageFile));
			}
		}
	}
}
