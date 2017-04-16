using System;
using System.Globalization;
using System.Linq;
using System.Text;
using CF.Library.Core.Bootstrap;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.DiscAdviser;
using CF.MusicLibrary.BL.Objects;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.AlbumAdviser
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Instances of the class are created by DI Container")]
	internal class AlbumAdviserApplicationLogic : IApplicationLogic
	{
		private const int AdvisedDiscsNumber = 30;

		private readonly IMusicLibraryRepository libraryRepository;
		private readonly IArtistLibraryBuilder artistLibraryBuilder;
		private readonly IDiscAdviser discAdviser;

		public AlbumAdviserApplicationLogic(IMusicLibraryRepository libraryRepository, IArtistLibraryBuilder artistLibraryBuilder, IDiscAdviser discAdviser)
		{
			if (libraryRepository == null)
			{
				throw new ArgumentNullException(nameof(libraryRepository));
			}
			if (artistLibraryBuilder == null)
			{
				throw new ArgumentNullException(nameof(artistLibraryBuilder));
			}
			if (discAdviser == null)
			{
				throw new ArgumentNullException(nameof(discAdviser));
			}

			this.libraryRepository = libraryRepository;
			this.artistLibraryBuilder = artistLibraryBuilder;
			this.discAdviser = discAdviser;
		}

		public int Run(string[] args)
		{
			while (true)
			{
				Console.WriteLine("Loading library...");
				DiscLibrary discLibrary = libraryRepository.LoadLibrary();
				ArtistLibrary artistLibrary = artistLibraryBuilder.Build(discLibrary);

				StringBuilder dbStatistics = new StringBuilder();
				dbStatistics.AppendLine("DB statistics:");
				dbStatistics.AppendLine();
				dbStatistics.AppendLine(Current($"\tArtists totally:\t{artistLibrary.Artists.Count}"));
				dbStatistics.AppendLine(Current($"\tArtists totally:\t{artistLibrary.Discs.Count()}"));
				dbStatistics.AppendLine(Current($"\tSongs totally:\t\t{artistLibrary.Songs.Count()}"));
				Console.WriteLine(dbStatistics.ToString());

				foreach (var disc in discAdviser.AdviseNextDiscs(artistLibrary, AdvisedDiscsNumber))
				{
					Console.WriteLine(Current($"[{FormatPlaybackTime(disc.Artist.LastPlaybackTime)}] [{FormatPlaybackTime(disc.LastPlaybackTime)}]  {disc.Artist.Name,-30} {disc.Title}"));
					Console.ReadKey(true);
				}
			}
		}

		private static string FormatPlaybackTime(DateTime? playbackDateTime)
		{
			return playbackDateTime?.ToString("yyyy.MM.dd", CultureInfo.InvariantCulture) ?? "  Never   ";
		}
	}
}
