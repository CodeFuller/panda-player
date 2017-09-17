using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CF.MusicLibrary.BL.Objects
{
	public class DiscLibrary : IEnumerable<Disc>
	{
		private List<Disc> discs;

		private readonly Func<Task<IEnumerable<Disc>>> discsLoader;

		/// <summary>
		/// Collection of library discs, excluding deleted.
		/// </summary>
		public IReadOnlyCollection<Disc> Discs
		{
			get
			{
				if (discs == null)
				{
					throw new InvalidOperationException("Library has not been loaded yet");
				}

				return discs.Where(d => !d.IsDeleted).ToList();
			}
		}

		/// <summary>
		/// Collection of all discs, including deleted, if they were loaded.
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

		public IEnumerable<Artist> Artists => Songs.Select(s => s.Artist).Where(a => a != null).Distinct();

		public IEnumerable<Genre> Genres => Songs.Select(s => s.Genre).Where(g => g != null).Distinct();

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

		public void RemoveDisc(Disc disc)
		{
			discs.Remove(disc);
		}

		private void FillPlaybacksPassed()
		{
			int playbacksPassed = 0;
			foreach (var disc in discs.OrderByDescending(d => d.LastPlaybackTime))
			{
				disc.PlaybacksPassed = disc.LastPlaybackTime.HasValue ? playbacksPassed++ : Int32.MaxValue;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<Disc> GetEnumerator()
		{
			return Discs.GetEnumerator();
		}
	}
}
