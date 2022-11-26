using System;
using System.Linq;
using PandaPlayer.DiscAdder.Extensions;
using PandaPlayer.DiscAdder.Interfaces;
using PandaPlayer.DiscAdder.ViewModels.Interfaces;
using PandaPlayer.DiscAdder.ViewModels.SourceContent;

namespace PandaPlayer.DiscAdder.Internal
{
	internal class SourceContentChecker : ISourceContentChecker
	{
		public void SetContentCorrectness(IReferenceContentViewModel referenceContent, IActualContentViewModel actualContent)
		{
			_ = referenceContent ?? throw new ArgumentNullException(nameof(referenceContent));
			_ = actualContent ?? throw new ArgumentNullException(nameof(actualContent));

			foreach (var (referenceDisc, actualDisc) in referenceContent.ExpectedDiscs.OuterZip(actualContent.Discs))
			{
				SetDiscsCorrectness(referenceDisc, actualDisc);
			}
		}

		private static void SetDiscsCorrectness(ReferenceDiscTreeItem referenceDisc, ActualDiscTreeItem actualDisc)
		{
			if (referenceDisc == null)
			{
				actualDisc.MarkWholeDiscAsIncorrect();
				return;
			}

			if (actualDisc == null)
			{
				referenceDisc.MarkWholeDiscAsIncorrect();
				return;
			}

			var referenceSongs = referenceDisc.ExpectedSongs;
			var actualSongs = actualDisc.Songs;

			foreach (var (referenceSong, currentSong) in referenceSongs.OuterZip(actualSongs))
			{
				SetSongsCorrectness(referenceSong, currentSong);
			}

			if (referenceSongs.Count != actualSongs.Count || referenceDisc.ExpectedDirectoryPath != actualDisc.DiscDirectory)
			{
				referenceDisc.ContentIsIncorrect = actualDisc.ContentIsIncorrect = true;
			}
			else
			{
				referenceDisc.ContentIsIncorrect = referenceSongs.Any(s => s.ContentIsIncorrect);
				actualDisc.ContentIsIncorrect = actualSongs.Any(s => s.ContentIsIncorrect);
			}
		}

		private static void SetSongsCorrectness(ReferenceSongTreeItem referenceSong, ActualSongTreeItem actualSong)
		{
			if (referenceSong == null)
			{
				actualSong.ContentIsIncorrect = true;
			}
			else if (actualSong == null)
			{
				referenceSong.ContentIsIncorrect = true;
			}
			else
			{
				// Do not use Path.ChangeExtension() for setting extension.
				// This approach will fail for songs with '.' in title, e.g. "Guilty All The Same (feat. Rakim)"
				var matchesTitleWithTrack = actualSong.FileName == $"{referenceSong.ExpectedTitleWithTrackNumber}.mp3";
				var matchesTitleWithoutTrack = actualSong.FileName == $"{referenceSong.ExpectedTitle}.mp3";
				actualSong.ContentIsIncorrect = referenceSong.ContentIsIncorrect = !(matchesTitleWithTrack || matchesTitleWithoutTrack);
			}
		}
	}
}
