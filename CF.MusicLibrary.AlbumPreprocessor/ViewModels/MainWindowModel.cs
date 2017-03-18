using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CF.Library.Core.Facades;
using CF.MusicLibrary.AlbumPreprocessor.Interfaces;
using CF.MusicLibrary.AlbumPreprocessor.ParsingContent;
using CF.MusicLibrary.AlbumPreprocessor.ParsingSong;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using static System.FormattableString;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels
{
	public class MainWindowModel : ViewModelBase
	{
		public static string Title => "Album Preprocessor";

		private readonly IFileSystemFacade fileSystemFacade;
		private readonly IAlbumContentParser albumContentParser;
		private readonly IAlbumContentComparer albumContentComparer;

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

		public MainWindowModel()
		{
			//	CF TEMP: Use DI
			fileSystemFacade = new FileSystemFacade();
			albumContentParser = new AlbumContentParser(new InputContentSplitter(), new EthalonAlbumParser(new EthalonSongParser()));
			albumContentComparer = new AlbumContentComparer();

			EthalonAlbums = new AlbumTreeViewModel(this);
			CurrentAlbums = new AlbumTreeViewModel(this);

			//	CF TEMP: Use DI
			RawEthalonAlbums = new EthalonContentViewModel(fileSystemFacade);
			RawEthalonAlbums.PropertyChanged += OnRawEthalonAlbumsPropertyChanged;
			//	Should be called after creation of RawEthalonAlbums because
			//	loading raw ethalong albums triggers OnRawEthalonAlbumsPropertyChanged() that updates EthalonAlbums.
			RawEthalonAlbums.LoadRawEthalonAlbumsContent();

			LoadCurrentAlbums();

			ReloadRawContentCommand = new RelayCommand(ReloadRawContent);
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

		private void OnRawEthalonAlbumsPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			UpdateAlbums(EthalonAlbums, albumContentParser.Parse(RawEthalonAlbums.Content));
		}

		public void LoadCurrentAlbums()
		{
			string albumsDirectory = ConfigurationManager.AppSettings["DefaultContentDirectory"];
			AlbumCrawler crawler = new AlbumCrawler(new SongFileFilter());
			var albums = crawler.LoadAlbums(albumsDirectory).ToList();

			UpdateAlbums(CurrentAlbums, albums);
		}

		private void UpdateAlbums(AlbumTreeViewModel albums, IEnumerable<AlbumContent> newAlbums)
		{
			albums.SetAlbums(newAlbums);
			SetContentCorrectness();
			ContentIsIncorrect = EthalonAlbums.ContentIsIncorrect || CurrentAlbums.ContentIsIncorrect;
		}

		private void SetContentCorrectness()
		{
			albumContentComparer.SetAlbumsCorrectness(EthalonAlbums, CurrentAlbums);
		}
	}
}
