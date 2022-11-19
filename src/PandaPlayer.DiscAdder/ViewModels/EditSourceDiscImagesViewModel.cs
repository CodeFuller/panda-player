using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PandaPlayer.Core.Models;
using PandaPlayer.DiscAdder.Interfaces;
using PandaPlayer.DiscAdder.ViewModels.Interfaces;
using PandaPlayer.DiscAdder.ViewModels.ViewModelItems;
using PandaPlayer.Shared.Images;

namespace PandaPlayer.DiscAdder.ViewModels
{
	internal class EditSourceDiscImagesViewModel : ViewModelBase, IEditSourceDiscImagesViewModel
	{
		private readonly IContentCrawler contentCrawler;
		private readonly IObjectFactory<IImageFile> imageFileFactory;

		private IReadOnlyCollection<DiscViewItem> Discs { get; set; }

		public string Name => "Edit Disc Images";

		public bool DataIsReady => ImageItems.All(im => im.ImageIsValid);

		public ObservableCollection<DiscImageViewItem> ImageItems { get; }

		public ICommand RefreshContentCommand { get; }

		public EditSourceDiscImagesViewModel(IContentCrawler contentCrawler, IObjectFactory<IImageFile> imageFileFactory)
		{
			this.contentCrawler = contentCrawler ?? throw new ArgumentNullException(nameof(contentCrawler));
			this.imageFileFactory = imageFileFactory ?? throw new ArgumentNullException(nameof(imageFileFactory));

			ImageItems = new ObservableCollection<DiscImageViewItem>();

			RefreshContentCommand = new RelayCommand(RefreshContent);
		}

		public void Load(IEnumerable<DiscViewItem> discs)
		{
			Discs = discs.Where(d => d.ExistingDisc == null).ToList();

			LoadImages();
		}

		internal void RefreshContent()
		{
			LoadImages();
			RaisePropertyChanged(nameof(DataIsReady));
		}

		private void LoadImages()
		{
			ImageItems.Clear();

			foreach (var discItem in Discs)
			{
				var discImages = contentCrawler.LoadDiscImages(discItem.SourcePath).ToList();
				if (discImages.Count > 1)
				{
					throw new InvalidOperationException($"Disc '{discItem.SourcePath}' contains multiple images. Only one image per disc (cover image) is currently supported.");
				}

				var imageFile = imageFileFactory.CreateInstance();
				if (discImages.Count == 1)
				{
					imageFile.Load(discImages.Single(), false);
				}

				ImageItems.Add(new DiscImageViewItem(discItem, DiscImageType.Cover, imageFile));
			}
		}
	}
}
