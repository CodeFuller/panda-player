using System;
using System.Globalization;
using System.Linq;
using System.Text;
using CF.Library.Core.Bootstrap;
using CF.MusicLibrary.BL.DiscAdviser;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.AlbumAdviser
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Instances of the class are created by DI Container")]
	internal class AlbumAdviserApplicationLogic : IApplicationLogic
	{
		private const int AdvisedDiscsNumber = 30;

		private readonly IMusicLibrary library;
		private readonly IDiscAdviser discAdviser;

		public AlbumAdviserApplicationLogic(IMusicLibrary library, IDiscAdviser discAdviser)
		{
			if (library == null)
			{
				throw new ArgumentNullException(nameof(library));
			}
			if (discAdviser == null)
			{
				throw new ArgumentNullException(nameof(discAdviser));
			}

			this.library = library;
			this.discAdviser = discAdviser;
		}

		public int Run(string[] args)
		{
			while (true)
			{
				Console.WriteLine("Loading library...");
				library.LoadAsync().Wait();
				ArtistLibrary artistLibrary = library.ArtistLibrary;

				StringBuilder dbStatistics = new StringBuilder();
				dbStatistics.AppendLine("DB statistics:");
				dbStatistics.AppendLine();
				dbStatistics.AppendLine(Current($"\tArtists totally:\t{artistLibrary.Artists.Count}"));
				dbStatistics.AppendLine(Current($"\tAlbums totally:\t\t{artistLibrary.Discs.Count()}"));
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
