using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CF.Library.Core.Facades;
using CF.MusicLibrary.AlbumPreprocessor.Interfaces;
using CF.MusicLibrary.AlbumPreprocessor.ParsingContent;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using static System.FormattableString;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels
{
	public class MainWindowModel : ViewModelBase
	{
		public static string Title => "Album Preprocessor";

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

		public MainWindowModel(IFileSystemFacade fileSystemFacade, IAlbumContentParser albumContentParser, IAlbumContentComparer albumContentComparer)
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

			this.albumContentParser = albumContentParser;
			this.albumContentComparer = albumContentComparer;

			EthalonAlbums = new AlbumTreeViewModel(this);
			CurrentAlbums = new AlbumTreeViewModel(this);

			RawEthalonAlbums = new EthalonContentViewModel(fileSystemFacade);
			RawEthalonAlbums.PropertyChanged += OnRawEthalonAlbumsPropertyChanged;

			ReloadRawContentCommand = new RelayCommand(ReloadRawContent);
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
