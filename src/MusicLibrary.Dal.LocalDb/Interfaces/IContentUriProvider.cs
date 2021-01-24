using System;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Dal.LocalDb.Interfaces
{
	internal interface IContentUriProvider
	{
		Uri GetSongContentUri(SongModel song);

		Uri GetDiscImageUri(DiscImageModel discImage);
	}
}
