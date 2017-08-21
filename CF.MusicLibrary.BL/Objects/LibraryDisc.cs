using System;
using System.Collections.Generic;
using System.Linq;

namespace CF.MusicLibrary.BL.Objects
{
	public class LibraryDisc
	{
		/// <remarks>
		/// Value is null untill DiscLibrary.FillPassedPlaybacks() is called.
		/// </remarks>
		private int? playbacksPassed;

		public string Title => Disc.Title;

		public LibraryArtist Artist { get; }

		public int? Year => Disc.Year;

		public Genre Genre
		{
			get
			{
				List<Genre> songGenres = Songs.Select(s => s.Genre).Distinct().ToList();
				return songGenres.Count > 1 ? null : songGenres.FirstOrDefault();
			}
		}

		private Disc Disc { get; }

		public IEnumerable<Song> Songs => Disc.Songs;

		public DateTime? LastPlaybackTime => Songs.Select(s => s.LastPlaybackTime).Min();

		public bool WasPlayedEver => LastPlaybackTime != null;

		public double Rating => Songs.Select(s => (double)s.SafeRating).Average();

		/// <summary>
		/// How long ago this disc was played.
		/// </summary>
		/// <remarks>
		/// Value is Int32.MaxValue if disc has not been played yet.
		/// </remarks>
		public int PlaybacksPassed
		{
			get
			{
				if (!playbacksPassed.HasValue)
				{
					throw new InvalidOperationException("Library playbacks should be calculated first");
				}

				return playbacksPassed.Value;
			}

			set { playbacksPassed = value; }
		}

		public Uri Uri => Disc.Uri;

		public LibraryDisc(LibraryArtist artist, Disc disc)
		{
			if (artist == null)
			{
				throw new ArgumentNullException(nameof(artist));
			}
			if (disc == null)
			{
				throw new ArgumentNullException(nameof(disc));
			}

			Artist = artist;
			Disc = disc;
		}
	}
}
