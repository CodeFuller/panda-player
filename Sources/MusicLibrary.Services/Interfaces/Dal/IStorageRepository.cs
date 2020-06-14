using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;
using MusicLibrary.Services.Diagnostic;
using MusicLibrary.Services.Diagnostic.Inconsistencies;
using MusicLibrary.Services.Tagging;

namespace MusicLibrary.Services.Interfaces.Dal
{
	public interface IStorageRepository
	{
		Task CreateDisc(DiscModel disc, CancellationToken cancellationToken);

		Task UpdateDiscTreeTitle(DiscModel oldDisc, DiscModel newDisc, CancellationToken cancellationToken);

		Task AddSong(SongModel song, Stream songContent, CancellationToken cancellationToken);

		Task UpdateSongTreeTitle(SongModel oldSong, SongModel newSong, CancellationToken cancellationToken);

		Task UpdateSong(SongModel song, CancellationToken cancellationToken);

		Task DeleteSong(SongModel song, CancellationToken cancellationToken);

		Task AddDiscImage(DiscImageModel image, Stream imageContent, CancellationToken cancellationToken);

		Task DeleteDiscImage(DiscImageModel image, CancellationToken cancellationToken);

		Task CreateFolder(ShallowFolderModel folder, CancellationToken cancellationToken);

		Task DeleteFolder(ShallowFolderModel folder, CancellationToken cancellationToken);

		Task<SongTagData> GetSongTagData(SongModel song, CancellationToken cancellationToken);

		Task<IEnumerable<SongTagType>> GetSongTagTypes(SongModel song, CancellationToken cancellationToken);

		Task CheckStorage(LibraryCheckFlags checkFlags, IEnumerable<ShallowFolderModel> folders, IEnumerable<DiscModel> discs, IOperationProgress progress, Action<LibraryInconsistency> inconsistenciesHandler, CancellationToken cancellationToken);
	}
}
