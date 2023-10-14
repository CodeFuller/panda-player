using PandaPlayer.Core.Models;

namespace PandaPlayer.UnitTests.Helpers
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
