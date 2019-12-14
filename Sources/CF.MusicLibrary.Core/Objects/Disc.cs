using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CF.Library.Core.Extensions;
using CF.MusicLibrary.Core.Objects.Images;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.Core.Objects
{
	public class Disc
	{
		public int Id { get; set; }

		public Artist Artist => RepresentativeSongs.Select(s => s.Artist).UniqueOrDefault();

		public Genre Genre => RepresentativeSongs.Select(s => s.Genre).UniqueOrDefault();

		public short? Year => RepresentativeSongs.Select(s => s.Year).UniqueOrDefault();

		/// <summary>
		/// Gets or sets Disc Title.
		/// </summary>
		/// <example>
		/// The Classical Conspiracy (Live) (CD 1)
		/// </example>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets Album Title.
		/// </summary>
		/// <example>
		/// The Classical Conspiracy
		/// </example>>
		public string AlbumTitle { get; set; }

		public Uri Uri { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Support property for Uri field. It's required because Uri type is not supported by used Data Provider.")]
		public string DiscUri
		{
			get => Uri.ToString();
			set => Uri = new Uri(value, UriKind.Relative);
		}

		public DateTime? LastPlaybackTime
		{
			get
			{
				var analyzedSongs = ((IsDeleted || AllSongs.All(s => !s.IsDeleted)) ? AllSongs : Songs).ToList();
				return analyzedSongs.Any(s => s.LastPlaybackTime == null) ? null : analyzedSongs.Select(s => s.LastPlaybackTime).Min();
			}
		}

		public int? PlaybacksPassed { get; set; }

		public IEnumerable<Song> Songs => AllSongs.Where(s => !s.IsDeleted);

		public IEnumerable<Song> AllSongs => SongsUnordered?
			.OrderBy(s => s.TrackNumber == null)
			.ThenBy(s => s.TrackNumber)
			.ThenBy(s => s.Artist == null)
			.ThenBy(s => s.Artist?.Name)
			.ThenBy(s => s.Title);

		public ICollection<Song> SongsUnordered { get; set; } = new Collection<Song>();

		public IEnumerable<Song> RepresentativeSongs => IsDeleted ? AllSongs : Songs;

		public ICollection<DiscImage> Images { get; } = new Collection<DiscImage>();

		public DiscImage CoverImage
		{
			get => Images.SingleOrDefault(im => im.ImageType == DiscImageType.Cover);
			set
			{
				if (value != null && value.ImageType != DiscImageType.Cover)
				{
					throw new InvalidOperationException(Current($"Added disc image is not a cover image ({value.ImageType})"));
				}

				Images.Remove(CoverImage);

				if (value != null)
				{
					Images.Add(value);
				}
			}
		}

		public bool IsDeleted => !Songs.Any();
	}
}
