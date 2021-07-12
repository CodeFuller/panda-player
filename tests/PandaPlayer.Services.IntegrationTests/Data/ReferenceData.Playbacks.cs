using System;
using System.Collections.Generic;
using System.Globalization;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.IntegrationTests.Data
{
	public partial class ReferenceData
	{
		public static ItemId NextPlaybackId => new("6");

		private void FillPlaybacks()
		{
			SongWithOptionalPropertiesFilled1.Playbacks = new List<PlaybackModel>
			{
				new()
				{
					Id = new ItemId("1"),
					PlaybackTime = DateTimeOffset.Parse("2021-03-19 13:35:02.2626013+03:00", CultureInfo.InvariantCulture),
				},
				new()
				{
					Id = new ItemId("4"),
					PlaybackTime = DateTimeOffset.Parse("2021-04-03 10:33:53.3517221+03:00", CultureInfo.InvariantCulture),
				},
			};

			SongWithOptionalPropertiesFilled2.Playbacks = new List<PlaybackModel>
			{
				new()
				{
					Id = new ItemId("2"),
					PlaybackTime = DateTimeOffset.Parse("2021-03-19 13:39:13.1718232+03:00", CultureInfo.InvariantCulture),
				},
				new()
				{
					Id = new ItemId("5"),
					PlaybackTime = DateTimeOffset.Parse("2021-04-03 10:37:42.1257252+03:00", CultureInfo.InvariantCulture),
				},
			};

			SongWithOptionalPropertiesMissing.Playbacks = new List<PlaybackModel>();

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
