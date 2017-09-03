using System;
using System.IO;
using System.Threading.Tasks;

namespace CF.MusicLibrary.BL.Interfaces
{
	public interface IMusicStorage
	{
		Task AddSongAsync(string sourceFileName, Uri songUri);

		Task SetAlbumCoverImage(Uri albumUri, string coverImagePath);

		FileInfo GetSongFile(Uri songUri);

		bool CheckSongContent(Uri songUri);
	}
}
