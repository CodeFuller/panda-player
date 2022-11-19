using System.IO;
using System.Threading.Tasks;

namespace PandaPlayer.Services.Media
{
	public class SongMediaInfoProvider : ISongMediaInfoProvider
	{
		public Task<SongMediaInfo> GetSongMediaInfo(string songFileName)
		{
			using var file = TagLib.File.Create(songFileName);

			var properties = file.Properties;
			var mediaInfo = new SongMediaInfo
			{
				Size = (int)new FileInfo(songFileName).Length,

				// TagLib returns bit rate in Kb/s, e.g. 320 for 320 Kb/s
				// Adjusting the value to b/s.
				Bitrate = 1000 * properties.AudioBitrate,
				Duration = properties.Duration,
			};

			return Task.FromResult(mediaInfo);
		}
	}
}
