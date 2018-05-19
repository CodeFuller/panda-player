using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers
{
	public class MaxUnlistenedSongTerm
	{
		public Rating Rating { get; set; }

		public int Days { get; set; }
	}
}
