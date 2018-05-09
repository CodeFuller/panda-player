using System;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface ILibraryStatisticsViewModel
	{
		int ArtistsNumber { get; }

		int DiscArtistsNumber { get; }

		int DiscsNumber { get; }

		int SongsNumber { get; }

		long StorageSize { get; }

		TimeSpan TotalDuration { get; }

		TimeSpan ListensDuration { get; }

		int ListensNumber { get; }

		double UnlistenedSongsPercentage { get; }

		double UnratedSongsPercentage { get; }

		double PercentageOfDiscsWithoutCoverImage { get; }
	}
}
