using MusicLibrary.Core.Models;

namespace MusicLibrary.DiscAdder.AddingToLibrary
{
	internal class AddedDisc
	{
		public DiscModel Disc { get; }

		public bool IsNewDisc { get; }

		public string SourcePath { get; }

		public AddedDisc(DiscModel disc, bool isNewDisc, string sourcePath)
		{
			Disc = disc;
			IsNewDisc = isNewDisc;
			SourcePath = sourcePath;
		}
	}
}
