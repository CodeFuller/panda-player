using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CF.MusicLibrary.Core.Objects
{
	public class DiscLibrary
	{
		private readonly Func<Task<IEnumerable<Disc>>> discsLoader;

		private List<Disc> discs;

		/// <summary>
		/// Gets collection of library discs, excluding deleted.
		/// </summary>
		public IEnumerable<Disc> Discs => AllDiscs.Where(disc => !disc.IsDeleted);

		/// <summary>
		/// Gets collection of all discs including deleted.
		/// </summary>
		public IReadOnlyCollection<Disc> AllDiscs
		{
			get
			{
				if (discs == null)
				{
					throw new InvalidOperationException("Library has not been loaded yet");
				}

				return discs;
			}
		}

		public IEnumerable<Song> Songs => Discs.SelectMany(d => d.Songs).Where(s => !s.IsDeleted);

		/// <summary>
		/// Gets collection of all songs including deleted.
		/// </summary>
		public IEnumerable<Song> AllSongs => AllDiscs.SelectMany(d => d.AllSongs);

		public IEnumerable<Artist> Artists => Songs.Select(s => s.Artist).Where(a => a != null).Distinct();

		public IEnumerable<Artist> AllArtists => AllSongs.Select(s => s.Artist).Where(a => a != null).Distinct();

		public IEnumerable<Genre> Genres => AllSongs.Select(s => s.Genre).Where(g => g != null).Distinct();

		internal DiscLibrary()
			: this(Enumerable.Empty<Disc>())
		{
		}

		public DiscLibrary(IEnumerable<Disc> libraryDiscs)
		{
			discs = libraryDiscs.ToList();
			FillPlaybacksPassed();
		}

		public DiscLibrary(Func<Task<IEnumerable<Disc>>> loader)
		{
			discsLoader = loader;
		}

		public async Task Load()
		{
			if (discsLoader == null)
			{
				throw new InvalidOperationException("Library loader is not set");
			}

			discs = (await discsLoader()).ToList();
			FillPlaybacksPassed();
		}

		private void FillPlaybacksPassed()
		{
			int playbacksPassed = 0;
			foreach (var disc in discs.OrderByDescending(d => d.LastPlaybackTime))
			{
				disc.PlaybacksPassed = disc.LastPlaybackTime.HasValue ? playbacksPassed++ : Int32.MaxValue;
			}
		}
	}
}
