using System;
using System.Linq;
using CF.MusicLibrary.DiscPreprocessor.Interfaces;
using CF.MusicLibrary.DiscPreprocessor.ViewModels.SourceContent;
using static System.FormattableString;

namespace CF.MusicLibrary.DiscPreprocessor
{
	public class DiscContentComparer : IDiscContentComparer
	{
		public void SetDiscsCorrectness(DiscTreeViewModel ethalonDiscs, DiscTreeViewModel currentDiscs)
		{
			if (ethalonDiscs == null)
			{
				throw new ArgumentNullException(nameof(ethalonDiscs));
			}
			if (currentDiscs == null)
			{
				throw new ArgumentNullException(nameof(currentDiscs));
			}

			for (var i = 0; i < Math.Max(ethalonDiscs.Discs.Count, currentDiscs.Discs.Count); ++i)
			{
				var ethalonDisc = i < ethalonDiscs.Discs.Count ? ethalonDiscs.Discs[i] : null;
				var currentDisc = i < currentDiscs.Discs.Count ? currentDiscs.Discs[i] : null;
				SetDiscsCorrectness(ethalonDisc, currentDisc);
			}
		}

		private static void SetDiscsCorrectness(DiscTreeViewItem ethalonDisc, DiscTreeViewItem currentDisc)
		{
			if (ethalonDisc == null && currentDisc == null)
			{
				throw new InvalidOperationException();
			}

			if (ethalonDisc == null)
			{
				MarkDiscSongsAsIncorrect(currentDisc);
			}
			else if (currentDisc == null)
			{
				MarkDiscSongsAsIncorrect(ethalonDisc);
			}
			else
			{
				var ethalonSongs = ethalonDisc.Songs.ToList();
				var currentSongs = currentDisc.Songs.ToList();
				for (var i = 0; i < Math.Max(ethalonSongs.Count, currentSongs.Count); ++i)
				{
					var ethalonSong = i < ethalonSongs.Count ? ethalonSongs[i] : null;
					var currentSong = i < currentSongs.Count ? currentSongs[i] : null;
					SetSongsCorrectness(i + 1, ethalonSong, currentSong);
				}

				if (ethalonSongs.Count != currentSongs.Count || ethalonDisc.DiscDirectory != currentDisc.DiscDirectory)
				{
					ethalonDisc.ContentIsIncorrect = currentDisc.ContentIsIncorrect = true;
				}
				else
				{
					ethalonDisc.ContentIsIncorrect = ethalonSongs.Any(s => s.ContentIsIncorrect);
					currentDisc.ContentIsIncorrect = currentSongs.Any(s => s.ContentIsIncorrect);
				}
			}
		}

		private static void SetSongsCorrectness(int songNumber, SongTreeViewItem ethalonSong, SongTreeViewItem currentSong)
		{
			if (ethalonSong == null && currentSong == null)
			{
				throw new InvalidOperationException();
			}

			if (ethalonSong == null)
			{
				currentSong.ContentIsIncorrect = true;
			}
			else if (currentSong == null)
			{
				ethalonSong.ContentIsIncorrect = true;
			}
			else
			{
				bool matchesTitleWithTrack = currentSong.Title == Invariant($"{songNumber:D2} - {ethalonSong.Title}.mp3");
				bool matchesTitleWithoutTrack = currentSong.Title == Invariant($"{ethalonSong.Title}.mp3");
				currentSong.ContentIsIncorrect = ethalonSong.ContentIsIncorrect = !(matchesTitleWithTrack || matchesTitleWithoutTrack);
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
