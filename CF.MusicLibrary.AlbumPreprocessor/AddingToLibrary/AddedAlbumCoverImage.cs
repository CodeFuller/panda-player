using System;

namespace CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary
{
	public class AddedAlbumCoverImage
	{
		public Uri AlbumStorageUri { get; }

		public string CoverImageFileName { get; }

		public AddedAlbumCoverImage(Uri albumStorageUri, string coverImageFileName)
		{
			AlbumStorageUri = albumStorageUri;
			CoverImageFileName = coverImageFileName;
		}
	}
}
