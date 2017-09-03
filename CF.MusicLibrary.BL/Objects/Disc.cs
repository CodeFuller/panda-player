using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using CF.Library.Core.Extensions;

namespace CF.MusicLibrary.BL.Objects
{
	public class Disc
	{
		[Key]
		public int Id { get; set; }

		[NotMapped]
		public Artist Artist
		{
			get
			{
				var artists = Songs.Select(s => s.Artist).Distinct().ToList();
				return artists.Count == 1 ? artists.Single() : null;
			}
		}

		public int? Year { get; set; }

		/// <example>
		/// The Classical Conspiracy (Live) (CD 1)
		/// </example>
		[Required]
		public string Title { get; set; }

		/// <example>
		/// The Classical Conspiracy
		/// </example>>
		public string AlbumTitle { get; set; }

		[NotMapped]
		public Uri Uri { get; set; }

		[Required]
		[Column("Uri")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Support property for Uri field. It's required because Uri type is not supported by used Data Provider.")]
		public string DiscUri
		{
			get { return Uri.ToString(); }
			set { Uri = new Uri(value, UriKind.Relative); }
		}

		[NotMapped]
		public ICollection<Song> Songs => SongsUnordered?.OrderBy(s => s.TrackNumber).ThenBy(s => s.Title).ToCollection();

		public ICollection<Song> SongsUnordered { get; set; }
	}
}
