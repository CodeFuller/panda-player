using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CF.MusicLibrary.DiscPreprocessor.Interfaces;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.DiscPreprocessor
{
	public class DiscCrawler : IDiscCrawler
	{
		private readonly ISongFileFilter songFileFilter;

		public DiscCrawler(ISongFileFilter songFileFilter)
		{
			if (songFileFilter == null)
			{
				throw new ArgumentNullException(nameof(songFileFilter));
			}

			this.songFileFilter = songFileFilter;
		}

		public IEnumerable<DiscContent> LoadDiscs(string discsDirectory)
		{
			return LoadDiscs(new DirectoryInfo(discsDirectory));
		}

		private IEnumerable<DiscContent> LoadDiscs(DirectoryInfo directoryInfo)
		{
			List<string> songFiles = directoryInfo.GetFiles().
				OrderBy(f => f.Name).
				Select(f => f.FullName).
				ToList();

			List<DiscContent> nestedDiscs = new List<DiscContent>();
			foreach (var subDirectory in directoryInfo.GetDirectories().OrderBy(x => x.Name))
			{
				nestedDiscs.AddRange(LoadDiscs(subDirectory));

				if (songFiles.Any() && nestedDiscs.Any())
				{
					throw new InvalidOperationException(Current($"Directory '{directoryInfo.FullName}' has both songs and discs. It's and invalid disc structure."));
				}
			}

			if (songFiles.Any())
			{
				return Enumerable.Repeat(new DiscContent(directoryInfo.FullName, songFiles.Where(songFileFilter.IsSongFile).Select(Path.GetFileName)), 1);
			}
			else
			{
				return nestedDiscs;
			}
		}
	}
}
