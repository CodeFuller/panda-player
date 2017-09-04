using System.IO;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Media;

namespace CF.MusicLibrary.Tagger
{
	public class SongMediaInfoProvider : ISongMediaInfoProvider
	{
		public async Task<SongMediaInfo> GetSongMediaInfo(string songFileName)
		{
			return await Task.Run(() =>
			{
				using (TagLib.File file = TagLib.File.Create(songFileName))
				{
					var properties = file.Properties;
					return new SongMediaInfo
					{
						Size = (int)new FileInfo(songFileName).Length,
						//	TagLib returns bitrate in Kb/s, e.g. 320 for 320 Kb/s
						//	Adjusting the value to b/s.
						Bitrate = 1000 * properties.AudioBitrate,
						Duration = properties.Duration,
					};
				}
			});
		}
	}
}
