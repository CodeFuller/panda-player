using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CF.Library.Core.Facades;
using CF.Library.Core.Interfaces;
using CF.MusicLibrary.AlbumPreprocessor.Events;
using CF.MusicLibrary.AlbumPreprocessor.Interfaces;
using CF.MusicLibrary.AlbumPreprocessor.ParsingContent;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using static System.FormattableString;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels
{
	public class MainWindowModel : ViewModelBase
	{
		public static string Title => "Album Preprocessor";

		private readonly IAlbumContentParser albumContentParser;
		private readonly IAlbumContentComparer albumContentComparer;
		private readonly IObjectFactory<AddToLibraryViewModel> addToLibraryViewModelFactory;

		private bool contentIsIncorrect;
		public bool ContentIsIncorrect
		{
			get { return contentIsIncorrect; }
			set
			{
				Set(ref contentIsIncorrect, value);
			}
		}

		public EthalonContentViewModel RawEthalonAlbums { get; }

		public AlbumTreeViewModel EthalonAlbums { get; }

		public AlbumTreeViewModel CurrentAlbums { get; }

		public ICommand ReloadRawContentCommand { get; }

		public ICommand AddToLibraryCommand { get; }

		public MainWindowModel(
			IFileSystemFacade fileSystemFacade,
			IAlbumContentParser albumContentParser,
			IAlbumContentComparer albumContentComparer,
			IObjectFactory<AddToLibraryViewModel> addToLibraryViewModelFactory)
		{
			if (fileSystemFacade == null)
			{
				throw new ArgumentNullException(nameof(fileSystemFacade));
			}
			if (albumContentParser == null)
			{
				throw new ArgumentNullException(nameof(albumContentParser));
			}
			if (albumContentComparer == null)
			{
				throw new ArgumentNullException(nameof(albumContentComparer));
			}
			if (addToLibraryViewModelFactory == null)
			{
				throw new ArgumentNullException(nameof(addToLibraryViewModelFactory));
			}

			this.albumContentParser = albumContentParser;
			this.albumContentComparer = albumContentComparer;
			this.addToLibraryViewModelFactory = addToLibraryViewModelFactory;

			EthalonAlbums = new AlbumTreeViewModel();
			CurrentAlbums = new AlbumTreeViewModel();

			string appDataPath = ConfigurationManager.AppSettings["AppDataPath"];
			RawEthalonAlbums = new EthalonContentViewModel(fileSystemFacade, appDataPath);
			RawEthalonAlbums.PropertyChanged += OnRawEthalonAlbumsPropertyChanged;

			ReloadRawContentCommand = new RelayCommand(ReloadRawContent);
			AddToLibraryCommand = new RelayCommand(AddToLibrary);

			Messenger.Default.Register<AlbumContentChangedEventArgs>(this, OnAlbumContentChanged);
		}

		private void OnAlbumContentChanged(AlbumContentChangedEventArgs message)
		{
			UpdateContentCorrectness();
		}

		public void LoadDefaultContent()
		{
			RawEthalonAlbums.LoadRawEthalonAlbumsContent();

			LoadCurrentAlbums();
		}

		private void ReloadRawContent()
		{
			var contentBuilder = new StringBuilder();
			foreach (var album in CurrentAlbums.Albums)
			{
				contentBuilder.Append(Invariant($"# {album.AlbumDirectory}\n\n"));
			}

			RawEthalonAlbums.Content = contentBuilder.ToString();
		}

		private async void AddToLibrary()
		{
			AddToLibraryViewModel addToLibraryViewModel = addToLibraryViewModelFactory.CreateInstance();
			await addToLibraryViewModel.AddAlbumsToLibrary(CurrentAlbums);
		}

		private void OnRawEthalonAlbumsPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			UpdateAlbums(EthalonAlbums, albumContentParser.Parse(RawEthalonAlbums.Content));
		}

		public void LoadCurrentAlbums()
		{
			string albumsDirectory = ConfigurationManager.AppSettings["WorkshopDirectory"];
			AlbumCrawler crawler = new AlbumCrawler(new SongFileFilter());
			var albums = crawler.LoadAlbums(albumsDirectory).ToList();

			UpdateAlbums(CurrentAlbums, albums);
		}

		private void UpdateAlbums(AlbumTreeViewModel albums, IEnumerable<AlbumContent> newAlbums)
		{
			albums.SetAlbums(newAlbums);
			UpdateContentCorrectness();
		}

		private void SetContentCorrectness()
		{
			albumContentComparer.SetAlbumsCorrectness(EthalonAlbums, CurrentAlbums);
		}

		private void UpdateContentCorrectness()
		{
			SetContentCorrectness();
			ContentIsIncorrect = EthalonAlbums.ContentIsIncorrect || CurrentAlbums.ContentIsIncorrect;
		}
	}
}
