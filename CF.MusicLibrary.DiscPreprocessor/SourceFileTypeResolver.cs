using System;
using System.IO;
using CF.MusicLibrary.Common.Images;
using CF.MusicLibrary.DiscPreprocessor.Interfaces;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.DiscPreprocessor
{
	public class SourceFileTypeResolver : ISourceFileTypeResolver
	{
		private readonly IDiscImageValidator discImageValidator;

		public SourceFileTypeResolver(IDiscImageValidator discImageValidator)
		{
			if (discImageValidator == null)
			{
				throw new ArgumentNullException(nameof(discImageValidator));
			}

			this.discImageValidator = discImageValidator;
		}

		public SourceFileType GetSourceFileType(string filePath)
		{
			if (Path.GetExtension(filePath) == ".mp3")
			{
				return SourceFileType.Song;
			}

			if (discImageValidator.IsSupportedFileFormat(filePath))
			{
				return SourceFileType.Image;
			}

			throw new InvalidOperationException(Current($"Failed to recognize type of source file '{filePath}'"));
		}
	}
}
