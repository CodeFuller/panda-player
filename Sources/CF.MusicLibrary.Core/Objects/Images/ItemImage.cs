using System;

namespace CF.MusicLibrary.Core.Objects.Images
{
	public class ItemImage
	{
		public int Id { get; set; }

		public Uri Uri { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Support property for Uri field. It's required because Uri type is not supported by used Data Provider.")]
		public string ImageUri
		{
			get { return Uri.ToString(); }
			set { Uri = new Uri(value, UriKind.Relative); }
		}

		public int FileSize { get; set; }

		public int? Checksum { get; set; }
	}
}
