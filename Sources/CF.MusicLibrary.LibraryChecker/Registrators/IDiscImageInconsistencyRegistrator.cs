﻿using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.Core.Objects.Images;

namespace CF.MusicLibrary.LibraryChecker.Registrators
{
	public interface IDiscImageInconsistencyRegistrator
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		void RegisterInconsistency_DiscCoverIsTooSmall(Disc disc, ImageInfo imageInfo);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		void RegisterInconsistency_DiscCoverIsTooBig(Disc disc, ImageInfo imageInfo);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		void RegisterInconsistency_ImageFileIsTooBig(Disc disc, ImageInfo imageInfo);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		void RegisterInconsistency_ImageHasUnsupportedFormat(Disc disc, ImageInfo imageInfo);
	}
}