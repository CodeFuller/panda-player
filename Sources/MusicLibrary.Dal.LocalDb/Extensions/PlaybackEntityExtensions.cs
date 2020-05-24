using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Entities;

namespace MusicLibrary.Dal.LocalDb.Extensions
{
	internal static class PlaybackEntityExtensions
	{
		public static PlaybackModel ToModel(this PlaybackEntity playback)
		{
			return new PlaybackModel
			{
				Id = playback.Id.ToItemId(),
				PlaybackTime = playback.PlaybackTime,
			};
		}
	}
}
