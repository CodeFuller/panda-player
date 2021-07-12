using System;
using System.Linq;
using PandaPlayer.DiscAdder.Interfaces;
using PandaPlayer.DiscAdder.ViewModels.SourceContent;

namespace PandaPlayer.DiscAdder.Internal
{
	internal class DiscContentComparer : IDiscContentComparer
	{
		public void SetDiscsCorrectness(DiscTreeViewModel referenceDiscs, DiscTreeViewModel currentDiscs)
		{
			if (referenceDiscs == null)
			{
				throw new ArgumentNullException(nameof(referenceDiscs));
			}

			if (currentDiscs == null)
			{
				throw new ArgumentNullException(nameof(currentDiscs));
			}

			for (var i = 0; i < Math.Max(referenceDiscs.Discs.Count, currentDiscs.Discs.Count); ++i)
			{
				var referenceDisc = i < referenceDiscs.Discs.Count ? referenceDiscs.Discs[i] : null;
				var currentDisc = i < currentDiscs.Discs.Count ? currentDiscs.Discs[i] : null;
				SetDiscsCorrectness(referenceDisc, currentDisc);
			}
		}

		private static void SetDiscsCorrectness(DiscTreeViewItem referenceDisc, DiscTreeViewItem currentDisc)
		{
			if (referenceDisc == null && currentDisc == null)
			{
				throw new InvalidOperationException();
			}

			if (referenceDisc == null)
			{
				MarkDiscSongsAsIncorrect(currentDisc);
			}
			else if (currentDisc == null)
			{
				MarkDiscSongsAsIncorrect(referenceDisc);
			}
			else
			{
				var referenceSongs = referenceDisc.Songs.ToList();
				var currentSongs = currentDisc.Songs.ToList();
				for (var i = 0; i < Math.Max(referenceSongs.Count, currentSongs.Count); ++i)
				{
					var referenceSong = i < referenceSongs.Count ? referenceSongs[i] : null;
					var currentSong = i < currentSongs.Count ? currentSongs[i] : null;
					SetSongsCorrectness(i + 1, referenceSong, currentSong);
				}

				if (referenceSongs.Count != currentSongs.Count || referenceDisc.DiscDirectory != currentDisc.DiscDirectory)
				{
					referenceDisc.ContentIsIncorrect = currentDisc.ContentIsIncorrect = true;
				}
				else
				{
					referenceDisc.ContentIsIncorrect = referenceSongs.Any(s => s.ContentIsIncorrect);
					currentDisc.ContentIsIncorrect = currentSongs.Any(s => s.ContentIsIncorrect);
				}
			}
		}

		private static void SetSongsCorrectness(int songNumber, SongTreeViewItem referenceSong, SongTreeViewItem currentSong)
		{
			if (referenceSong == null && currentSong == null)
			{
				throw new InvalidOperationException();
			}

			if (referenceSong == null)
			{
				currentSong.ContentIsIncorrect = true;
			}
			else if (currentSong == null)
			{
				referenceSong.ContentIsIncorrect = true;
			}
			else
			{
				var matchesTitleWithTrack = currentSong.Title == $"{songNumber:D2} - {referenceSong.Title}.mp3";
				var matchesTitleWithoutTrack = currentSong.Title == $"{referenceSong.Title}.mp3";
				currentSong.ContentIsIncorrect = referenceSong.ContentIsIncorrect = !(matchesTitleWithTrack || matchesTitleWithoutTrack);
			}
		}

		private static void MarkDiscSongsAsIncorrect(DiscTreeViewItem disc)
		{
			if (disc == null)
			{
				throw new ArgumentNullException(nameof(disc));
			}

			foreach (var song in disc.Songs)
			{
				song.ContentIsIncorrect = true;
			}

			disc.ContentIsIncorrect = true;
		}
	}
}
