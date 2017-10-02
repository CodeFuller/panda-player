using System;
using System.Collections.Generic;
using System.Linq;

namespace CF.MusicLibrary.DiscPreprocessor.MusicStorage
{
	public class AddedDiscInfo
	{
		public short? Year { get; set; }

		public string Title { get; set; }

		public DsicType DiscType { get; set; }

		public string SourcePath { get; set; }

		public Uri UriWithinStorage { get; set; }

		public bool HasArtist => DiscType == DsicType.ArtistDisc;

		private string artist;
		/// <summary>
		/// This value is filled only for type of ArtistDisc.
		/// </summary>
		public string Artist
		{
			get
			{
				if (!HasArtist)
				{
					throw new InvalidOperationException("Atist property is accessible only for artist discs");
				}

				return artist;
			}

			set
			{
				if (!HasArtist)
				{
					throw new InvalidOperationException("Atist property is accessible only for artist discs");
				}

				artist = value;
			}
		}

		private readonly List<AddedSongInfo> songs;
		public IReadOnlyCollection<AddedSongInfo> Songs => songs;

		public AddedDiscInfo(IEnumerable<AddedSongInfo> songs)
		{
			this.songs = songs.ToList();
		}
	}
}
