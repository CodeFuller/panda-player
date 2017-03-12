using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using CF.Library.Core.Extensions;
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
		private string parsedEthalonAlbumsContent;
		private List<AlbumContent> parsedEthalonAlbums;

		private readonly IAlbumContentParser albumContentParser;
		private readonly IAlbumContentComparer albumContentComparer;

		public string RawEthalonAlbumsContent
		{
			get { return rawEthalonAlbumsContent; }
			set
			{
				Set(ref rawEthalonAlbumsContent, value);
				UpdateParsedEthalonAlbums();
			}
		}

		public string ParsedEthalonAlbumsContent
		{
			get { return parsedEthalonAlbumsContent; }
			set
			{
				Set(ref parsedEthalonAlbumsContent, value);
			}
		}

		private Collection<AlbumTreeViewItem> currentAlbums;
		public Collection<AlbumTreeViewItem> CurrentAlbums
		{
			get { return currentAlbums; }
			set
			{
				Set(ref currentAlbums, value);
			}
		}

		public MainWindowModel()
		{
			//	CF TEMP: Use DI
			albumContentParser = new AlbumContentParser(new InputContentSplitter(), new EthalonAlbumParser(new EthalonSongParser()));
			albumContentComparer = new AlbumContentComparer();
		}

		private void UpdateParsedEthalonAlbums()
		{
			parsedEthalonAlbums = albumContentParser.Parse(RawEthalonAlbumsContent).ToList();
			var parsedAlbumsContent = parsedEthalonAlbums.
				SelectMany(album =>
				{
					List<string> parsedAlbumContent = new List<string>();
					parsedAlbumContent.Add(album.AlbumDirectory);
					parsedAlbumContent.AddRange(album.Songs.Select(s => s.Title));
					parsedAlbumContent.Add(String.Empty);
					return parsedAlbumContent;
				});
			ParsedEthalonAlbumsContent = String.Join("\n", parsedAlbumsContent);

			SetContentCorrectness();
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

			CurrentAlbums = albums.Select(a => new AlbumTreeViewItem(this, a)).ToCollection();

			SetContentCorrectness();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameter use makes sense in method semantics.")]
		public void ReloadAlbum(AlbumTreeViewItem album)
		{
			LoadCurrentAlbums();
		}

		private void SetContentCorrectness()
		{
			if (CurrentAlbums != null)
			{
				albumContentComparer.SetAlbumsCorrectness(parsedEthalonAlbums.ToCollection(), CurrentAlbums.ToCollection());
			}
		}
	}
}
