using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CF.MusicLibrary.BL.Objects
{
	public class LibraryArtist
	{
		private readonly List<LibraryDisc> discs = new List<LibraryDisc>();

		public string Id { get; }

		public string Name { get; }

		public IReadOnlyCollection<LibraryDisc> Discs => discs;

		public int PlaybacksPassed => Discs.Select(d => d.PlaybacksPassed).Min();

		public DateTime? LastPlaybackTime => Discs.Select(d => d.LastPlaybackTime).Max();

		public LibraryArtist(Artist artist)
		{
			if (artist == null)
			{
				throw new ArgumentNullException(nameof(artist));
			}

			Id = artist.Id.ToString(CultureInfo.InvariantCulture);
			Name = artist.Name;
		}

		public LibraryArtist(string id, string name)
		{
			Id = id;
			Name = name;
		}

		public void AddDisc(LibraryDisc disc)
		{
			discs.Add(disc);
		}
	}
}
