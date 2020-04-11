using System;

namespace CF.MusicLibrary.Core.Objects.Images
{
	public class ItemImage
	{
		public int Id { get; set; }

		public Uri Uri { get; set; }

#pragma warning disable CA1056 // Uri properties should not be strings
		public string ImageUri
#pragma warning restore CA1056 // Uri properties should not be strings
		{
			get => Uri.ToString();
			set => Uri = new Uri(value, UriKind.Relative);
		}

		public int FileSize { get; set; }

		public int? Checksum { get; set; }
	}
}
