using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CF.Library.Core.Configuration;
using CF.Library.Core.Facades;
using CF.MusicLibrary.AlbumPreprocessor.Events;
using CF.MusicLibrary.AlbumPreprocessor.Interfaces;
using CF.MusicLibrary.AlbumPreprocessor.ParsingContent;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels.Interfaces;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels.SourceContent;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using static System.FormattableString;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels
{
	public class EditSourceContentViewModel : ViewModelBase, IEditSourceContentViewModel
	{
		public string Name => "Edit Source Content";

		private readonly IAlbumContentParser albumContentParser;
		private readonly IAlbumContentComparer albumContentComparer;

		public EthalonContentViewModel RawEthalonAlbums { get; }

		public AlbumTreeViewModel EthalonAlbums { get; }

		public AlbumTreeViewModel CurrentAlbums { get; }

		public ICommand ReloadRawContentCommand { get; }

		private bool dataIsReady;
		public bool DataIsReady
		{
			get { return dataIsReady; }
			set { Set(ref dataIsReady, value); }
		}

		public EditSourceContentViewModel(IAlbumContentParser albumContentParser, IAlbumContentComparer albumContentComparer, IFileSystemFacade fileSystemFacade)
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

			EthalonAlbums = new AlbumTreeViewModel();
			CurrentAlbums = new AlbumTreeViewModel();

			string appDataPath = AppSettings.GetRequiredValue<string>("AppDataPath");
			RawEthalonAlbums = new EthalonContentViewModel(fileSystemFacade, appDataPath);
			RawEthalonAlbums.PropertyChanged += OnRawEthalonAlbumsPropertyChanged;

			ReloadRawContentCommand = new RelayCommand(ReloadRawContent);

			Messenger.Default.Register<AlbumContentChangedEventArgs>(this, OnAlbumContentChanged);
		}

		public void LoadDefaultContent()
		{
			RawEthalonAlbums.LoadRawEthalonAlbumsContent();

			LoadCurrentAlbums();
		}

		private void OnAlbumContentChanged(AlbumContentChangedEventArgs message)
		{
			UpdateContentCorrectness();
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
			DataIsReady = !EthalonAlbums.ContentIsIncorrect && !CurrentAlbums.ContentIsIncorrect;
		}
	}
}
