using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CF.MusicLibrary.AlbumPreprocessor.Interfaces;

namespace CF.MusicLibrary.AlbumPreprocessor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly IAlbumContentParser albumContentParser;

		public MainWindow()
		{
			InitializeComponent();

			//	CF TEMP
			albumContentParser = new AlbumContentParser(new InputContentSplitter(), new EthalonAlbumParser(new EthalonSongParser()));
		}

		private void ButtonBrowseAlbumsDir_Click(object sender, RoutedEventArgs e)
		{

		}

		private void ButtonLoadCurrentAlbums_Click(object sender, RoutedEventArgs e)
		{
			AlbumCrawler crawler = new AlbumCrawler(new SongFileFilter());

			var rawEthalonAlbumsBuilder = new StringBuilder();
			var currentAlbumsContentBuilder = new StringBuilder();
			foreach (AlbumContent album in crawler.LoadAlbums(@"d:\Music.other\set_tags\do"))
			{
				rawEthalonAlbumsBuilder.Append($"{album.AlbumDirectory}\n\n");

				currentAlbumsContentBuilder.AppendLine(album.AlbumDirectory);
				foreach (SongContent song in album.Songs)
				{
					currentAlbumsContentBuilder.AppendLine(song.Title);
				}
				currentAlbumsContentBuilder.AppendLine();
			}

			TextBoxRawEthalonAlbums.Text = rawEthalonAlbumsBuilder.ToString();
			TextBoxCurrentAlbumsContent.Text = currentAlbumsContentBuilder.ToString();
		}

		private void ButtonParseRawEthalonAlbums_Click(object sender, RoutedEventArgs e)
		{
			var parsedAlbumsContent = albumContentParser.Parse(TextBoxRawEthalonAlbums.Text).
				SelectMany(album =>
				{
					List<string> parsedAlbumContent = new List<string>();
					parsedAlbumContent.Add(album.AlbumDirectory);
					parsedAlbumContent.AddRange(album.Songs.Select(s => s.Title));
					parsedAlbumContent.Add(String.Empty);
					return parsedAlbumContent;
				});
			TextBoxParsedEthalonAlbums.Text = String.Join("\n", parsedAlbumsContent);
		}

		private void ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			List<TextBox> scrolledControls = new List<TextBox>
			{
				TextBoxRawEthalonAlbums,
				TextBoxParsedEthalonAlbums,
				TextBoxCurrentAlbumsContent,
			};

			scrolledControls.RemoveAll(x => x == sender);

			foreach (var control in scrolledControls)
			{
				control.ScrollToVerticalOffset(e.VerticalOffset);
			}
		}

		private void AlbumContentChanged(object sender, TextChangedEventArgs e)
		{
			//	var ethalonAlbums = albumContentParser.Parse(TextBoxParsedEthalonAlbums.Text);
			//	var currentAlbums = albumContentParser.Parse(TextBoxCurrentAlbumsContent.Text);
		}
	}
}
