using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CF.MusicLibrary.BL.Objects
{
	public class Disc
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Title { get; set; }

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

		public ICollection<Song> Songs { get; set; } = new HashSet<Song>();
	}
}
