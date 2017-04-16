using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CF.Library.Core.Extensions;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Objects;
using static System.FormattableString;

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

			if (x.OrderNumber == 0 && y.OrderNumber == 0)
			{
				return String.Compare(x.Uri.ToString(), y.Uri.ToString(), StringComparison.OrdinalIgnoreCase);
			}

			if (x.OrderNumber != 0 && y.OrderNumber != 0)
			{
				return x.OrderNumber.CompareTo(y.OrderNumber);
			}

			throw new InvalidOperationException(Invariant($"Inconsistency in songs ordering: [{x.OrderNumber}, {x.Uri}] vs [{y.OrderNumber}, {y.Uri}]"));
		}
	}

	/// <summary>
	/// Implementation of ILibraryBuilder for MediaMonkey database.
	/// </summary>
	public class LibraryBuilder : ILibraryBuilder
	{
		private readonly List<Song> songs = new List<Song>();

		/// <summary>
		/// Implementation for ILibraryBuilder.AddSong().
		/// </summary>
		public void AddSong(Song song)
		{
			songs.Add(song);
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
				OrderDicsSongs(disc);
				disc.Id = discId++;
				discs.Add(disc);
			}

			return new DiscLibrary(discs);
		}

		private Dictionary<string, Disc> BuildDiscs()
		{
			var discs = new Dictionary<string, Disc>();
			foreach (var song in songs)
			{
				var discPath = Path.GetDirectoryName(song.Uri.LocalPath);
				Disc disc;
				if (!discs.TryGetValue(discPath, out disc))
				{
					disc = new Disc
					{
						Title = BuildDiscTitle(discPath),
						Uri = new Uri(discPath),
					};
					discs.Add(discPath, disc);
				}
				disc.Songs.Add(song);
			}

			return discs;
		}

		private static void OrderDicsSongs(Disc disc)
		{
			//	Ordering songs
			disc.Songs = disc.Songs.OrderBy(x => x, new SongOrderComparer()).ToCollection();

			//	Settings songs order
			short currOrderNumber = 1;
			foreach (var song in disc.Songs)
			{
				if (song.OrderNumber == 0)
				{
					song.OrderNumber = currOrderNumber;
				}
				else
				{
					currOrderNumber = song.OrderNumber;
				}

				++currOrderNumber;
			}
		}

		private static string BuildDiscTitle(string discPath)
		{
			string[] dirs = discPath.Split(Path.DirectorySeparatorChar);
			if (dirs.Length < 1)
			{
				throw new InvalidOperationException(Invariant($"Bad disc path: '{discPath}'"));
			}
			return dirs[dirs.Length - 1];
		}
	}
}
