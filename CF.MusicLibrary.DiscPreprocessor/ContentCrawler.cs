using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CF.MusicLibrary.DiscPreprocessor.Interfaces;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.DiscPreprocessor
{
	public class ContentCrawler : IContentCrawler
	{
		private readonly ISourceFileTypeResolver sourceFileTypeResolver;

		public ContentCrawler(ISourceFileTypeResolver sourceFileTypeResolver)
		{
			if (sourceFileTypeResolver == null)
			{
				throw new ArgumentNullException(nameof(sourceFileTypeResolver));
			}

			this.sourceFileTypeResolver = sourceFileTypeResolver;
		}

		public IEnumerable<DiscContent> LoadDiscs(string discsDirectory)
		{
			return LoadDiscs(new DirectoryInfo(discsDirectory));
		}

		private IEnumerable<DiscContent> LoadDiscs(DirectoryInfo directoryInfo)
		{
			List<string> files = directoryInfo.GetFiles().
				OrderBy(f => f.Name).
				Select(f => f.FullName).
				ToList();

			List<DiscContent> nestedDiscs = new List<DiscContent>();
			foreach (var subDirectory in directoryInfo.GetDirectories().OrderBy(x => x.Name))
			{
				nestedDiscs.AddRange(LoadDiscs(subDirectory));

				if (files.Any() && nestedDiscs.Any())
				{
					throw new InvalidOperationException(Current($"Directory '{directoryInfo.FullName}' contains both directories and files. It is an invalid disc structure."));
				}
			}

			if (files.Any())
			{
				var songFiles = files.Where(f => sourceFileTypeResolver.GetSourceFileType(f) == SourceFileType.Song).Select(Path.GetFileName);
				return Enumerable.Repeat(new DiscContent(directoryInfo.FullName, songFiles), 1);
			}
			else
			{
				return nestedDiscs;
			}
		}

		public IEnumerable<string> LoadDiscImages(string discDirectory)
		{
			return GetDirectoryFiles(new DirectoryInfo(discDirectory))
				.Where(f => sourceFileTypeResolver.GetSourceFileType(f) == SourceFileType.Image);
		}

		private static List<string> GetDirectoryFiles(DirectoryInfo directoryInfo)
		{
			return directoryInfo.GetFiles().
				OrderBy(f => f.Name).
				Select(f => f.FullName).
				ToList();
		}
	}
}
