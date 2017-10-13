using System;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.DiscPreprocessor.Interfaces;

namespace CF.MusicLibrary.DiscPreprocessor
{
	public class SongFileFilter : ISongFileFilter
	{
		private readonly IDiscArtFileStorage discArtFileStorage;

		public SongFileFilter(IDiscArtFileStorage discArtFileStorage)
		{
			if (discArtFileStorage == null)
			{
				throw new ArgumentNullException(nameof(discArtFileStorage));
			}

			this.discArtFileStorage = discArtFileStorage;
		}

		public bool IsSongFile(string filePath)
		{
			return !discArtFileStorage.IsCoverImageFile(filePath);
		}
	}
}
