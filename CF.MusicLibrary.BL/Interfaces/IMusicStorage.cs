using System;
using System.Threading.Tasks;

namespace CF.MusicLibrary.BL.Interfaces
{
	public interface IMusicStorage
	{
		Task AddSongAsync(string sourceFileName, Uri songUri);

		Task SetAlbumCoverImage(Uri albumUri, string coverImagePath);
	}
}
