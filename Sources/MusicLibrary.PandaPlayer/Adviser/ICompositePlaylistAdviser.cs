namespace MusicLibrary.PandaPlayer.Adviser
{
	public interface ICompositePlaylistAdviser : IPlaylistAdviser
	{
		void RegisterAdvicePlayback(AdvisedPlaylist advise);
	}
}
