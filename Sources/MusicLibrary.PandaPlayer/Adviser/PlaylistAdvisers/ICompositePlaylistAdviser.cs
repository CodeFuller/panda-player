namespace MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers
{
	public interface ICompositePlaylistAdviser : IPlaylistAdviser
	{
		void RegisterAdvicePlayback(AdvisedPlaylist advise);
	}
}
