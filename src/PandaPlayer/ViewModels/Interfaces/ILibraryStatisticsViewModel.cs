using System;
using System.Threading;
using System.Threading.Tasks;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface ILibraryStatisticsViewModel
	{
		int ArtistsNumber { get; }

		int DiscArtistsNumber { get; }

		int DiscsNumber { get; }

		int SongsNumber { get; }

		long StorageSize { get; }

		TimeSpan SongsDuration { get; }

		TimeSpan PlaybacksDuration { get; }

		int PlaybacksNumber { get; }

		double UnheardSongsPercentage { get; }

		double UnratedSongsPercentage { get; }

		double PercentageOfDiscsWithoutCoverImage { get; }

		Task Load(CancellationToken cancellationToken);
	}
}
