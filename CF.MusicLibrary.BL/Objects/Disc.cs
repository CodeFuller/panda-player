using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CF.MusicLibrary.BL.Objects
{
	public class Disc
	{
		public int Id { get; set; }

		public Artist Artist
		{
			get
			{
				var artists = Songs.Select(s => s.Artist).Distinct().ToList();
				return artists.Count == 1 ? artists.Single() : null;
			}
		}

		public Genre Genre
		{
			get
			{
				var genres = Songs.Select(s => s.Genre).Distinct().ToList();
				return genres.Count == 1 ? genres.Single() : null;
			}
		}

		public short? Year
		{
			get
			{
				var years = Songs.Select(s => s.Year).Distinct().ToList();
				return years.Count == 1 ? years.Single() : null;
			}
		}

		/// <example>
		/// The Classical Conspiracy (Live) (CD 1)
		/// </example>
		public string Title { get; set; }

		/// <example>
		/// The Classical Conspiracy
		/// </example>>
		public string AlbumTitle { get; set; }

		public Uri Uri { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Support property for Uri field. It's required because Uri type is not supported by used Data Provider.")]
		public string DiscUri
		{
			get { return Uri.ToString(); }
			set { Uri = new Uri(value, UriKind.Relative); }
		}

		public DateTime? LastPlaybackTime => Songs.Any(s => s.LastPlaybackTime == null) ? null : Songs.Select(s => s.LastPlaybackTime).Min();

		public int? PlaybacksPassed { get; set; }

		public IEnumerable<Song> Songs => AllSongs.Where(s => !s.IsDeleted);

		public IEnumerable<Song> AllSongs => SongsUnordered?.OrderBy(s => s.TrackNumber).ThenBy(s => s.Title);

		public ICollection<Song> SongsUnordered { get; set; } = new Collection<Song>();

		public bool IsDeleted => Songs.All(s => s.IsDeleted);
	}
}
