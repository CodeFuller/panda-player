using System;
using System.Collections.Generic;
using System.Linq;

namespace CF.MusicLibrary.AlbumPreprocessor.MusicStorage
{
	public class AlbumInfo
	{
		public int? Year { get; set; }

		/// <summary>
		/// Album title including year, including "2015 - The Best Of"
		/// </summary>
		public string Title { get; set; }

		public AlbumType AlbumType { get; set; }

		public string PathWithinStorage { get; set; }

		public string NameInStorage { get; set; }

		private string artist;
		/// <summary>
		/// This value is filled only for type of ArtistAlbum.
		/// </summary>
		public string Artist
		{
			get
			{
				if (AlbumType != AlbumType.ArtistAlbum)
				{
					throw new InvalidOperationException("Atist property is accessible only for artist albums");
				}

				return artist;
			}

			set
			{
				if (AlbumType != AlbumType.ArtistAlbum)
				{
					throw new InvalidOperationException("Atist property is accessible only for artist albums");
				}

				artist = value;
			}
		}

		private readonly List<SongInfo> songs;
		public IReadOnlyCollection<SongInfo> Songs => songs;

		public AlbumInfo(IEnumerable<SongInfo> songs)
		{
			this.songs = songs.ToList();
		}
	}
}
