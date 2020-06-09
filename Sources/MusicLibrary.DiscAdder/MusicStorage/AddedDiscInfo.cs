using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicLibrary.DiscAdder.MusicStorage
{
	public class AddedDiscInfo
	{
		public short? Year { get; set; }

		public string DiscTitle { get; set; }

		public string TreeTitle { get; set; }

		public string AlbumTitle { get; set; }

		public DsicType DiscType { get; set; }

		public string SourcePath { get; set; }

		public IReadOnlyCollection<string> DestinationFolderPath { get; set; }

		public bool HasArtist => DiscType == DsicType.ArtistDisc;

		private string artist;

		/// <summary>
		/// Gets or sets the Disc Artist.
		/// </summary>
		/// <remarks>
		/// This value is filled only for type of ArtistDisc.
		/// </remarks>
		public string Artist
		{
			get
			{
				if (!HasArtist)
				{
					throw new InvalidOperationException("Artist property is accessible only for artist discs");
				}

				return artist;
			}

			set
			{
				if (!HasArtist)
				{
					throw new InvalidOperationException("Artist property is accessible only for artist discs");
				}

				artist = value;
			}
		}

		public IReadOnlyCollection<AddedSongInfo> Songs { get; }

		public AddedDiscInfo(IEnumerable<AddedSongInfo> songs)
		{
			Songs = songs.ToList();
		}
	}
}
