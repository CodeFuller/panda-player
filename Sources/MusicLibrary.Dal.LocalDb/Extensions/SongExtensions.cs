using System;
using System.Linq;
using CF.Library.Core.Extensions;
using MusicLibrary.Core;
using MusicLibrary.Core.Objects;
using MusicLibrary.Dal.LocalDb.Interfaces;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Dal.LocalDb.Extensions
{
	internal static class SongExtensions
	{
		public static SongModel ToModel(this Song song, DiscModel disc, IDataStorage dataStorage)
		{
			var songModel = new SongModel
			{
				Id = song.Id.ToItemId(),
				Title = song.Title,
				TreeTitle = new ItemUriParts(song.Uri).Last(),
				TrackNumber = song.TrackNumber,
				Duration = song.Duration,
				Disc = disc,
				Artist = song.Artist?.ToModel(),
				Genre = song.Genre?.ToModel(),
				Rating = song.Rating != null ? ConvertRating(song.Rating.Value) : null,
				BitRate = song.Bitrate,
				Size = song.FileSize,
				Checksum = song.Checksum.HasValue ? (uint)song.Checksum.Value : (uint?)null,
				LastPlaybackTime = song.LastPlaybackTime,
				PlaybacksCount = song.PlaybacksCount,
				ContentUri = dataStorage.TranslateInternalUri(song.Uri),
			};

			var playbacks = song.Playbacks
				.Select(p => new PlaybackModel
				{
					PlaybackTime = p.PlaybackTime,
				});

			songModel.Playbacks.AddRange(playbacks);

			return songModel;
		}

		private static RatingModel ConvertRating(Rating rating)
		{
			switch (rating)
			{
				case Rating.R1:
					return RatingModel.R1;
				case Rating.R2:
					return RatingModel.R2;
				case Rating.R3:
					return RatingModel.R3;
				case Rating.R4:
					return RatingModel.R4;
				case Rating.R5:
					return RatingModel.R5;
				case Rating.R6:
					return RatingModel.R6;
				case Rating.R7:
					return RatingModel.R7;
				case Rating.R8:
					return RatingModel.R8;
				case Rating.R9:
					return RatingModel.R9;
				case Rating.R10:
					return RatingModel.R10;

				default:
					throw new InvalidOperationException($"Unexpected rating value: {rating}");
			}
		}
	}
}
