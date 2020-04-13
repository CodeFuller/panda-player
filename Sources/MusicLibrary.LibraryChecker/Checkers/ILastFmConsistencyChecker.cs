using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.LibraryChecker.Checkers
{
	public interface ILastFMConsistencyChecker
	{
		Task CheckArtists(DiscLibrary library, CancellationToken cancellationToken);

		Task CheckAlbums(IEnumerable<Disc> discs, CancellationToken cancellationToken);

		Task CheckSongs(IEnumerable<Song> songs, CancellationToken cancellationToken);
	}
}
