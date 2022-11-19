using PandaPlayer.Core.Models;

namespace PandaPlayer.Dal.LocalDb.Interfaces
{
	internal interface IContentUriProvider
	{
		void SetSongContentUri(SongModel song);

		void SetDiscImageUri(DiscImageModel discImage);
	}
}
