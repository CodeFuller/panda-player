﻿using System;
using System.Linq;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Entities;
using MusicLibrary.Dal.LocalDb.Interfaces;
using MusicLibrary.Dal.LocalDb.Internal;

namespace MusicLibrary.Dal.LocalDb.Extensions
{
	internal static class SongEntityExtensions
	{
		public static SongModel ToModel(this SongEntity song, DiscModel discModel, IDataStorage dataStorage)
		{
			var songModel = new SongModel
			{
				Id = song.Id.ToItemId(),
				Title = song.Title,
				TreeTitle = new ItemUriParts(song.Uri).Last(),
				TrackNumber = song.TrackNumber,
				Duration = TimeSpan.FromMilliseconds(song.DurationInMilliseconds),
				Disc = discModel,
				Artist = song.Artist?.ToModel(),
				Genre = song.Genre?.ToModel(),
				Rating = song.Rating != null ? ConvertRating(song.Rating.Value) : null,
				BitRate = song.BitRate,
				Size = song.FileSize,
				Checksum = song.Checksum.HasValue ? (uint)song.Checksum.Value : (uint?)null,
				LastPlaybackTime = song.LastPlaybackTime,
				PlaybacksCount = song.PlaybacksCount,
				Playbacks = song.Playbacks?.Select(p => p.ToModel()).ToList(),
				ContentUri = dataStorage.TranslateInternalUri(song.Uri),
				DeleteDate = song.DeleteDate,
			};

			return songModel;
		}

		private static RatingModel ConvertRating(int rating)
		{
			switch (rating)
			{
				case 1:
					return RatingModel.R1;
				case 2:
					return RatingModel.R2;
				case 3:
					return RatingModel.R3;
				case 4:
					return RatingModel.R4;
				case 5:
					return RatingModel.R5;
				case 6:
					return RatingModel.R6;
				case 7:
					return RatingModel.R7;
				case 8:
					return RatingModel.R8;
				case 9:
					return RatingModel.R9;
				case 10:
					return RatingModel.R10;

				default:
					throw new InvalidOperationException($"Unexpected rating value: {rating}");
			}
		}
	}
}