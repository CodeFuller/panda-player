using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.Common.DiscArt;

namespace CF.MusicLibrary.LibraryChecker.Registrators
{
	public interface IDiscArtInconsistencyRegistrator
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		void RegisterInconsistency_DiscCoverIsTooSmall(Disc disc, DiscArtImageInfo imageInfo);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		void RegisterInconsistency_DiscCoverIsTooBig(Disc disc, DiscArtImageInfo imageInfo);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		void RegisterInconsistency_ImageFileIsTooBig(Disc disc, DiscArtImageInfo imageInfo);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		void RegisterInconsistency_ImageHasUnsupportedFormat(Disc disc, DiscArtImageInfo imageInfo);
	}
}
