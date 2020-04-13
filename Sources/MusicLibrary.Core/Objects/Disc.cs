﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CF.Library.Core.Extensions;
using MusicLibrary.Core.Objects.Images;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.Core.Objects
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
		/// The Classical Conspiracy (Live) (CD 1).
		/// </example>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets Album Title.
		/// </summary>
		/// <example>
		/// The Classical Conspiracy.
		/// </example>>
		public string AlbumTitle { get; set; }

		public Uri Uri { get; set; }

#pragma warning disable CA1056 // Uri properties should not be strings
		public string DiscUri
#pragma warning restore CA1056 // Uri properties should not be strings
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

#pragma warning disable CA2227 // Collection properties should be read only - Setter is required for EF Core
		public ICollection<Song> SongsUnordered { get; set; } = new Collection<Song>();
#pragma warning restore CA2227 // Collection properties should be read only

		public IEnumerable<Song> RepresentativeSongs => IsDeleted ? AllSongs : Songs;

#pragma warning disable CA2227 // Collection properties should be read only - Setter is required for EF Core
		public ICollection<DiscImage> Images { get; set; } = new Collection<DiscImage>();
#pragma warning restore CA2227 // Collection properties should be read only

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