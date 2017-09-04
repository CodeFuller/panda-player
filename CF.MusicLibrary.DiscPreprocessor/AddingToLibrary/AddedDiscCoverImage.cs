using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.DiscPreprocessor.AddingToLibrary
{
	public class AddedDiscCoverImage
	{
		public Disc Disc { get; }

		public string CoverImageFileName { get; }

		public AddedDiscCoverImage(Disc disc, string coverImageFileName)
		{
			Disc = disc;
			CoverImageFileName = coverImageFileName;
		}
	}
}
