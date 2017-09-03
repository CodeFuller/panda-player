using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using CF.Library.Core.Exceptions;
using CF.Library.Core.Extensions;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Objects;
using static System.FormattableString;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.Dal.MediaMonkey
{
	internal class SongOrderComparer : IComparer<Song>
	{
		public int Compare(Song x, Song y)
		{
			if (x == null)
			{
				throw new ArgumentNullException(nameof(x));
			}
			if (y == null)
			{
				throw new ArgumentNullException(nameof(y));
			}

			if (ReferenceEquals(x, y))
			{
				return 0;
			}

			if (x.TrackNumber == null && y.TrackNumber == null)
			{
				return String.Compare(x.Uri.ToString(), y.Uri.ToString(), StringComparison.OrdinalIgnoreCase);
			}

			if (x.TrackNumber != null && y.TrackNumber != null && x.TrackNumber != y.TrackNumber)
			{
				return x.TrackNumber.Value.CompareTo(y.TrackNumber.Value);
			}

			throw new InvalidOperationException(Invariant($"Inconsistency in songs ordering: [{x.TrackNumber}, {x.Uri}] vs [{y.TrackNumber}, {y.Uri}]"));
		}
	}

	internal class SongWithAlbumTitle
	{
		public Song Song { get; set; }

		public string AlbumTitle { get; set; }

		public SongWithAlbumTitle(Song song, string albumTitle)
		{
			Song = song;
			AlbumTitle = albumTitle;
		}
	}

	/// <summary>
	/// Implementation of ILibraryBuilder for MediaMonkey database.
	/// </summary>
	public class LibraryBuilder : ILibraryBuilder
	{
		private static readonly Regex AlbumDataRegex = new Regex(@"^(\d{4}) - (.+)$", RegexOptions.Compiled);

		private readonly List<SongWithAlbumTitle> songs = new List<SongWithAlbumTitle>();

		/// <summary>
		/// Implementation for ILibraryBuilder.AddSong().
		/// </summary>
		public void AddSong(Song song, string albumTitle)
		{
			songs.Add(new SongWithAlbumTitle(song, albumTitle));
		}

		/// <summary>
		/// Implementation for ILibraryBuilder.Build().
		/// </summary>
		public DiscLibrary Build()
		{
			var discsDictionary = BuildDiscs();

			List<Disc> discs = new List<Disc>();
			int discId = 1;
			foreach (var disc in discsDictionary.Values.OrderBy(d => d.Uri.ToString()))
			{
				disc.SongsUnordered = disc.Songs.OrderBy(x => x, new SongOrderComparer()).ToCollection();
				disc.Id = discId++;
				discs.Add(disc);
			}

			return new DiscLibrary(discs);
		}

		public void Clear()
		{
			songs.Clear();
		}

		private Dictionary<Uri, Disc> BuildDiscs()
		{
			var discs = new Dictionary<Uri, Disc>();
			foreach (var song in songs)
			{
				var discUri = song.Song.Uri.RemoveLastSegment();
				Disc disc;
				if (!discs.TryGetValue(discUri, out disc))
				{
					disc = new Disc
					{
						Year = GetDiscYear(discUri),
						Title = GetDiscTitle(discUri),
						AlbumTitle = song.AlbumTitle,
						Uri = discUri,
					};
					discs.Add(discUri, disc);
				}
				else
				{
					if (disc.AlbumTitle != song.AlbumTitle)
					{
						throw new InvalidInputDataException(Current($"Album title differ within the same disc: '{disc.AlbumTitle}' != '{song.AlbumTitle}'"));
					}
				}
				disc.Songs.Add(song.Song);
			}

			return discs;
		}

		private static string GetRawDiscTitle(Uri discUri)
		{
			var title = discUri.ToString().Split('/').LastOrDefault();
			if (title == null)
			{
				throw new InvalidOperationException(Invariant($"Bad disc URI: '{discUri}'"));
			}

			return title;
		}

		private static int? GetDiscYear(Uri discUri)
		{
			var rawTitle = GetRawDiscTitle(discUri);
			var match = AlbumDataRegex.Match(rawTitle);
			return match.Success ? (int?)Int32.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture) : null;
		}

		private static string GetDiscTitle(Uri discUri)
		{
			var rawTitle = GetRawDiscTitle(discUri);
			var match = AlbumDataRegex.Match(rawTitle);
			return match.Success ? match.Groups[2].Value : rawTitle;
		}
	}
}
