using PandaPlayer.Core.Models;

namespace PandaPlayer.UnitTests.Extensions
{
	internal static class SongModelExtensions
	{
		public static SongModel AddToDisc(this SongModel song, DiscModel disc)
		{
			disc.AddSong(song);
			return song;
		}
	}
}
