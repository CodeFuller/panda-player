using System;
using System.Collections.Generic;
using System.Globalization;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.IntegrationTests.Data
{
	public partial class ReferenceData
	{
		public static ItemId NextPlaybackId => new("4");

		private void FillPlaybacks()
		{
			SongWithOptionalPropertiesFilled.Playbacks = new List<PlaybackModel>
			{
				new()
				{
					Id = new ItemId("1"),
					PlaybackTime = DateTimeOffset.Parse("2021-03-19 13:35:02.2626013+03:00", CultureInfo.InvariantCulture),
				},
				new()
				{
					Id = new ItemId("2"),
					PlaybackTime = DateTimeOffset.Parse("2021-04-03 10:33:53.3517221+03:00", CultureInfo.InvariantCulture),
				},
			};

			SongWithOptionalPropertiesMissing.Playbacks = new List<PlaybackModel>();
			SongFromNullDisc.Playbacks = new List<PlaybackModel>();

			DeletedSong.Playbacks = new List<PlaybackModel>
			{
				new()
				{
					Id = new ItemId("3"),
					PlaybackTime = DateTimeOffset.Parse("2021-03-28 09:33:39.2582742+03:00", CultureInfo.InvariantCulture),
				},
			};
		}
	}
}
