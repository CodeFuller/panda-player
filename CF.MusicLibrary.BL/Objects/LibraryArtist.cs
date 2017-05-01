using System;
using System.Collections.Generic;
using System.Linq;

namespace CF.MusicLibrary.BL.Objects
{
	public class LibraryArtist
	{
		private readonly List<LibraryDisc> discs = new List<LibraryDisc>();

		public string Id { get; }

		public string Name { get; }

		/// <remarks>
		/// Is filled only for regular artists.
		/// </remarks>>
		public Uri StorageUri { get; }

		public IReadOnlyCollection<LibraryDisc> Discs => discs;

		public int PlaybacksPassed => Discs.Select(d => d.PlaybacksPassed).Min();

		public DateTime? LastPlaybackTime => Discs.Select(d => d.LastPlaybackTime).Max();

		public LibraryArtist(string id, string name) :
			this(id, name, null)
		{
		}

		public LibraryArtist(string id, string name, Uri storageUri)
		{
			Id = id;
			Name = name;
			StorageUri = storageUri;
		}

		public void AddDisc(LibraryDisc disc)
		{
			discs.Add(disc);
		}
	}
}
