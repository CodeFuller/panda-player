using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MusicLibrary.Core.Interfaces;
using MusicLibrary.Core.Objects;
using MusicLibrary.Core.Objects.Images;

namespace MusicLibrary.Library
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

		Task CheckDataConsistency(IEnumerable<Uri> expectedItemUris, IUriCheckScope checkScope, ILibraryStorageInconsistencyRegistrator registrator, bool fixFoundIssues);
	}
}
