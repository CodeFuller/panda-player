using PandaPlayer.Core.Models;
using PandaPlayer.Dal.LocalDb.Entities;

namespace PandaPlayer.Dal.LocalDb.Extensions
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
