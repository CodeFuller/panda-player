using System;
using System.Collections.Generic;
using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.Internal
{
	internal static class SongUpdater
	{
		private static readonly IReadOnlyDictionary<string, Action<SongModel, SongModel>> UpdateActions = new Dictionary<string, Action<SongModel, SongModel>>
		{
			{ nameof(SongModel.Title), (src, dst) => dst.Title = src.Title },
			{ nameof(SongModel.TrackNumber), (src, dst) => dst.TrackNumber = src.TrackNumber },
			{ nameof(SongModel.Artist), (src, dst) => dst.Artist = src.Artist },
			{ nameof(SongModel.Genre), (src, dst) => dst.Genre = src.Genre },
			{ nameof(SongModel.Rating), (src, dst) => dst.Rating = src.Rating },
			{ nameof(SongModel.LastPlaybackTime), (src, dst) => dst.LastPlaybackTime = src.LastPlaybackTime },
			{ nameof(SongModel.PlaybacksCount), (src, dst) => dst.PlaybacksCount = src.PlaybacksCount },
		};

		public static void UpdateSong(SongModel source, SongModel target, string propertyName)
		{
			if (!UpdateActions.TryGetValue(propertyName, out var updateAction))
			{
				throw new InvalidOperationException($"Can not update unknown Song property '{propertyName}'");
			}

			updateAction(source, target);
		}
	}
}
