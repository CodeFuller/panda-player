using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using CF.MusicLibrary.AlbumPreprocessor.Interfaces;
using CF.MusicLibrary.AlbumPreprocessor.ParsingContent;
using CF.MusicLibrary.AlbumPreprocessor.ParsingSong;
using GalaSoft.MvvmLight;
using static System.FormattableString;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels
{
	public class MainWindowModel : ViewModelBase
	{
		public static string Title => "Album Preprocessor";

		private string rawEthalonAlbumsContent;

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

		public string RawEthalonAlbumsContent
		{
			get { return rawEthalonAlbumsContent; }
			set
			{
				Set(ref rawEthalonAlbumsContent, value);
				UpdateAlbums(EthalonAlbums, albumContentParser.Parse(RawEthalonAlbumsContent));
			}
		}

		public AlbumTreeViewModel EthalonAlbums { get; }

		public AlbumTreeViewModel CurrentAlbums { get; }

		public MainWindowModel()
		{
			//	CF TEMP: Use DI
			albumContentParser = new AlbumContentParser(new InputContentSplitter(), new EthalonAlbumParser(new EthalonSongParser()));
			albumContentComparer = new AlbumContentComparer();

			EthalonAlbums = new AlbumTreeViewModel(this);
			CurrentAlbums = new AlbumTreeViewModel(this);
		}

		public void LoadCurrentAlbums()
		{
			string albumsDirectory = ConfigurationManager.AppSettings["DefaultContentDirectory"];
			AlbumCrawler crawler = new AlbumCrawler(new SongFileFilter());
			var albums = crawler.LoadAlbums(albumsDirectory).ToList();

			if (String.IsNullOrEmpty(RawEthalonAlbumsContent))
			{
				var rawEthalonAlbumsBuilder = new StringBuilder();
				foreach (AlbumContent album in albums)
				{
					rawEthalonAlbumsBuilder.Append(Invariant($"{album.AlbumDirectory}\n\n"));
				}

				RawEthalonAlbumsContent = rawEthalonAlbumsBuilder.ToString();
			}

			UpdateAlbums(CurrentAlbums, albums);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameter use makes sense in method semantics.")]
		public void ReloadAlbum(AlbumTreeViewItem album)
		{
			LoadCurrentAlbums();
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
