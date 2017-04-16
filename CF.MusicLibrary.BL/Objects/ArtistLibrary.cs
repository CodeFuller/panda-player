using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CF.MusicLibrary.BL.Objects
{
	/// <summary>
	/// Collection of Music Discs with groupping by Artist.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Suffix 'Library' is suitable in this context")]
	public class ArtistLibrary : IEnumerable<LibraryArtist>
	{
		private readonly List<LibraryArtist> artists;

		public IReadOnlyCollection<LibraryArtist> Artists => artists;

		public IEnumerable<LibraryDisc> Discs => Artists.SelectMany(a => a.Discs);

		public IEnumerable<Song> Songs => Discs.SelectMany(d => d.Songs);

		public ArtistLibrary(IEnumerable<LibraryArtist> libraryArtists)
		{
			artists = libraryArtists.ToList();
			FillPassedPlaybacks();
		}

		private void FillPassedPlaybacks()
		{
			int playbacksPassed = 0;
			foreach (var disc in Discs.OrderByDescending(d => d.LastPlaybackTime))
			{
				disc.PlaybacksPassed = disc.WasPlayedEver ? playbacksPassed++ : Int32.MaxValue;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<LibraryArtist> GetEnumerator()
		{
			return Artists.GetEnumerator();
		}
	}
}
