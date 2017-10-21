using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.Core.Objects.Images;

namespace CF.MusicLibrary.Library
{
	public interface IMusicLibraryStorage
	{
		Task StoreSong(string sourceSongFileName, Song song);

		Task<string> GetSongFile(Song song);

		Task<string> GetSongFileForWriting(Song song);

		Task UpdateSongContent(string sourceSongFileName, Song song);

		Task ChangeSongUri(Song song, Uri newSongUri);

		Task DeleteSong(Song song);

		Task ChangeDiscUri(Disc disc, Uri newDiscUri);

		Task StoreDiscImage(string sourceImageFileName, DiscImage discImage);

		Task<string> GetDiscImageFile(DiscImage discImage);

		Task DeleteDiscImage(DiscImage discImage);

		Task CheckDataConsistency(IEnumerable<Uri> expectedItemUris, ILibraryStorageInconsistencyRegistrator registrator, bool fixFoundIssues);
	}
}
