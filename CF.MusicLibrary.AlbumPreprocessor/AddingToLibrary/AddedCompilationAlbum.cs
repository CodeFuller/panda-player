using System;
using System.Collections.Generic;
using System.Linq;
using CF.MusicLibrary.AlbumPreprocessor.MusicStorage;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary
{
	public abstract class AddedCompilationAlbum : AddedAlbum
	{
		// Seal the method for calling it in constructor.
		public sealed override int? Year
		{
			get { return base.Year; }
			set
			{
				base.Year = value;
			}
		}

		public override bool DestinationUriIsEditable => false;

		public override IEnumerable<Uri> AvailableDestinationUris => Enumerable.Repeat(DestinationUri, 1);

		public override bool RequiredDataIsFilled => Genre != null;

		protected AddedCompilationAlbum(string sourcePath, AlbumInfo album, Uri destinationUri, IEnumerable<Genre> availableGenres) :
			base(sourcePath, album, availableGenres)
		{
			if (album == null)
			{
				throw new ArgumentNullException(nameof(album));
			}

			DestinationUri = destinationUri;
			Year = album.Year;
		}
	}
}
