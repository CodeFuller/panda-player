using System;
using System.Linq;
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

			foreach (var (referenceDisc, actualDisc) in referenceContent.Discs.OuterZip(actualContent.Discs))
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

			var referenceSongs = referenceDisc.Songs;
			var actualSongs = actualDisc.Songs;

			foreach (var (referenceSong, currentSong, songNumber) in referenceSongs.OuterZip(actualSongs).Select((pair, i) => (pair.First, pair.Second, i + 1)))
			{
				SetSongsCorrectness(songNumber, referenceSong, currentSong);
			}

			if (referenceSongs.Count != actualSongs.Count || referenceDisc.DiscDirectory != actualDisc.DiscDirectory)
			{
				referenceDisc.ContentIsIncorrect = actualDisc.ContentIsIncorrect = true;
			}
			else
			{
				referenceDisc.ContentIsIncorrect = referenceSongs.Any(s => s.ContentIsIncorrect);
				actualDisc.ContentIsIncorrect = actualSongs.Any(s => s.ContentIsIncorrect);
			}
		}

		private static void SetSongsCorrectness(int songNumber, ReferenceSongTreeItem referenceSong, ActualSongTreeItem actualSong)
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
				var matchesTitleWithTrack = actualSong.Title == $"{songNumber:D2} - {referenceSong.Title}.mp3";
				var matchesTitleWithoutTrack = actualSong.Title == $"{referenceSong.Title}.mp3";
				actualSong.ContentIsIncorrect = referenceSong.ContentIsIncorrect = !(matchesTitleWithTrack || matchesTitleWithoutTrack);
			}
		}
	}
}
