using System.Threading.Tasks;

namespace PandaPlayer.Services.Media
{
	internal class SongMediaInfoProvider : ISongMediaInfoProvider
	{
		public Task<SongMediaInfo> GetSongMediaInfo(string songFileName)
		{
			using var file = TagLib.File.Create(songFileName);

			var properties = file.Properties;
			var mediaInfo = new SongMediaInfo
			{
				// TagLib returns bit rate in Kb/s, e.g. 320 for 320 Kb/s
				// Adjusting the value to b/s.
				BitRate = 1000 * properties.AudioBitrate,
				Duration = properties.Duration,
			};

			return Task.FromResult(mediaInfo);
		}
	}
}
