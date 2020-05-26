using System;
using System.Linq;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Entities;
using MusicLibrary.Dal.LocalDb.Interfaces;

namespace MusicLibrary.Dal.LocalDb.Extensions
{
	internal static class SongEntityExtensions
	{
		public static SongModel ToModel(this SongEntity song, DiscModel discModel, IContentUriProvider contentUriProvider)
		{
			var model = new SongModel
			{
				Id = song.Id.ToItemId(),
				Title = song.Title,
				TreeTitle = song.TreeTitle,
				TrackNumber = song.TrackNumber,
				Duration = TimeSpan.FromMilliseconds(song.DurationInMilliseconds),
				Disc = discModel,
				Artist = song.Artist?.ToModel(),
				Genre = song.Genre?.ToModel(),
				Rating = song.Rating != null ? ConvertRating(song.Rating.Value) : null,
				BitRate = song.BitRate,
				Size = song.FileSize,
				Checksum = (uint?)song.Checksum,
				LastPlaybackTime = song.LastPlaybackTime,
				PlaybacksCount = song.PlaybacksCount,
				Playbacks = song.Playbacks?.Select(p => p.ToModel()).ToList(),
				DeleteDate = song.DeleteDate,
			};

			model.ContentUri = contentUriProvider.GetSongContentUri(model);

			return model;
		}

		public static SongEntity ToEntity(this SongModel song)
		{
			return new SongEntity
			{
				Id = song.Id.ToInt32(),
				DiscId = song.Disc.Id.ToInt32(),
				ArtistId = song.Artist?.Id.ToInt32(),
				TrackNumber = song.TrackNumber,
				Year = (short?)song.Disc.Year,
				Title = song.Title,
				TreeTitle = song.TreeTitle,
				GenreId = song.Genre?.Id.ToInt32(),
				DurationInMilliseconds = song.Duration.TotalMilliseconds,
				Rating = song.Rating != null ? ConvertRating(song.Rating) : (int?)null,
				FileSize = song.Size,
				Checksum = (int?)song.Checksum,
				BitRate = song.BitRate,
				LastPlaybackTime = song.LastPlaybackTime,
				PlaybacksCount = song.PlaybacksCount,
				DeleteDate = song.DeleteDate,
			};
		}

		private static RatingModel ConvertRating(int rating)
		{
			return rating switch
			{
				1 => RatingModel.R1,
				2 => RatingModel.R2,
				3 => RatingModel.R3,
				4 => RatingModel.R4,
				5 => RatingModel.R5,
				6 => RatingModel.R6,
				7 => RatingModel.R7,
				8 => RatingModel.R8,
				9 => RatingModel.R9,
				10 => RatingModel.R10,
				_ => throw new InvalidOperationException($"Unexpected rating value: {rating}")
			};
		}

		private static int ConvertRating(RatingModel rating)
		{
			if (rating == RatingModel.R1)
			{
				return 1;
			}

			if (rating == RatingModel.R2)
			{
				return 2;
			}

			if (rating == RatingModel.R3)
			{
				return 3;
			}

			if (rating == RatingModel.R4)
			{
				return 4;
			}

			if (rating == RatingModel.R5)
			{
				return 5;
			}

			if (rating == RatingModel.R6)
			{
				return 6;
			}

			if (rating == RatingModel.R7)
			{
				return 7;
			}

			if (rating == RatingModel.R8)
			{
				return 8;
			}

			if (rating == RatingModel.R9)
			{
				return 9;
			}

			if (rating == RatingModel.R10)
			{
				return 10;
			}

			throw new InvalidOperationException($"Unexpected rating value");
		}
	}
}
