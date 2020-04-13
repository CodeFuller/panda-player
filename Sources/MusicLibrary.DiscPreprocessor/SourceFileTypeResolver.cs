﻿using System;
using System.IO;
using MusicLibrary.Common.Images;
using MusicLibrary.DiscPreprocessor.Interfaces;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.DiscPreprocessor
{
	public class SourceFileTypeResolver : ISourceFileTypeResolver
	{
		private readonly IDiscImageValidator discImageValidator;

		public SourceFileTypeResolver(IDiscImageValidator discImageValidator)
		{
			this.discImageValidator = discImageValidator ?? throw new ArgumentNullException(nameof(discImageValidator));
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