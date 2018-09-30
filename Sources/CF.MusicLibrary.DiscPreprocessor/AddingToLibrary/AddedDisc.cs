using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.DiscPreprocessor.AddingToLibrary
{
	public class AddedDisc
	{
		public Disc Disc { get; }

		public bool IsNewDisc { get; }

		public string SourcePath { get; }

		public AddedDisc(Disc disc, bool isNewDisc, string sourcePath)
		{
			Disc = disc;
			IsNewDisc = isNewDisc;
			SourcePath = sourcePath;
		}
	}
}
