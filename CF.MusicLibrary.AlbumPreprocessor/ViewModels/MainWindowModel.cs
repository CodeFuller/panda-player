using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Core.Configuration;
using CF.Library.Core.Facades;
using CF.Library.Core.Interfaces;
using CF.MusicLibrary.AlbumPreprocessor.Events;
using CF.MusicLibrary.AlbumPreprocessor.Interfaces;
using CF.MusicLibrary.AlbumPreprocessor.ParsingContent;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using static System.FormattableString;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels
{
	public class MainWindowModel : ViewModelBase
	{
		public static string Title => "Album Preprocessor";

		private readonly IFileSystemFacade fileSystemFacade;
		private readonly IAlbumContentParser albumContentParser;
		private readonly IAlbumContentComparer albumContentComparer;
		private readonly IObjectFactory<IAddToLibraryViewModel> addToLibraryViewModelFactory;

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
			IObjectFactory<IAddToLibraryViewModel> addToLibraryViewModelFactory)
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

			this.fileSystemFacade = fileSystemFacade;
			this.albumContentParser = albumContentParser;
			this.albumContentComparer = albumContentComparer;
			this.addToLibraryViewModelFactory = addToLibraryViewModelFactory;

			EthalonAlbums = new AlbumTreeViewModel();
			CurrentAlbums = new AlbumTreeViewModel();

			string appDataPath = AppSettings.GetRequiredValue<string>("AppDataPath");
			RawEthalonAlbums = new EthalonContentViewModel(fileSystemFacade, appDataPath);
			RawEthalonAlbums.PropertyChanged += OnRawEthalonAlbumsPropertyChanged;

			ReloadRawContentCommand = new RelayCommand(ReloadRawContent);
			AddToLibraryCommand = new AsyncRelayCommand(AddToLibrary);

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

		public void ReloadRawContent()
		{
			var contentBuilder = new StringBuilder();
			foreach (var album in CurrentAlbums.Albums)
			{
				contentBuilder.Append(Invariant($"# {album.AlbumDirectory}\n\n"));
			}

			RawEthalonAlbums.Content = contentBuilder.ToString();
		}

		public async Task AddToLibrary()
		{
			IAddToLibraryViewModel addToLibraryViewModel = addToLibraryViewModelFactory.CreateInstance();
			bool added = await addToLibraryViewModel.AddAlbumsToLibrary(CurrentAlbums);

			if (added && AppSettings.GetRequiredValue<bool>("DeleteSourceContentAfterAdding"))
			{
				DeleteSourceDirTree();
			}
		}

		private void DeleteSourceDirTree()
		{
			foreach (var subDirectory in fileSystemFacade.EnumerateDirectories(AppSettings.GetRequiredValue<string>("WorkshopDirectory")))
			{
				List<string> files = new List<string>();
				FindDirectoryFiles(subDirectory, files);

				if (files.Any())
				{
					return;
				}

				fileSystemFacade.DeleteDirectory(subDirectory, true);
			}
		}

		private void FindDirectoryFiles(string directoryPath, List<string> files)
		{
			foreach (string subDirectory in fileSystemFacade.EnumerateDirectories(directoryPath))
			{
				FindDirectoryFiles(subDirectory, files);
			}

			foreach (string file in fileSystemFacade.EnumerateFiles(directoryPath))
			{
				files.Add(file);
			}
		}

		private void OnRawEthalonAlbumsPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			UpdateAlbums(EthalonAlbums, albumContentParser.Parse(RawEthalonAlbums.Content));
		}

		public void LoadCurrentAlbums()
		{
			AlbumCrawler crawler = new AlbumCrawler(new SongFileFilter());
			var albums = crawler.LoadAlbums(AppSettings.GetRequiredValue<string>("WorkshopDirectory")).ToList();

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
