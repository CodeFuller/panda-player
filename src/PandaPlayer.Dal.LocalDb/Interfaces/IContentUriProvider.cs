using System;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Dal.LocalDb.Interfaces
{
	internal interface IContentUriProvider
	{
		Uri GetSongContentUri(SongModel song);

		Uri GetDiscImageUri(DiscImageModel discImage);
	}
}
