using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CF.Library.Core.Bootstrap;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.DiscAdviser
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Instances of the class are created by DI Container")]
	internal class ApplicationLogic : IApplicationLogic
	{
		private const int AdvisedDiscsNumber = 30;

		private readonly IMusicLibraryReader library;
		private readonly IDiscAdviser discAdviser;

		public ApplicationLogic(IMusicLibraryReader library, IDiscAdviser discAdviser)
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
			RunAsync().Wait();

			return 0;
		}

		private async Task RunAsync()
		{
			while (true)
			{
				Console.WriteLine("Loading library...");

				DiscLibrary discLibrary = await library.Load(true);

				StringBuilder dbStatistics = new StringBuilder();
				dbStatistics.AppendLine("DB statistics:");
				dbStatistics.AppendLine();
				dbStatistics.AppendLine(Current($"\tArtists totally:\t{discLibrary.Artists.Count()}"));
				dbStatistics.AppendLine(Current($"\tDisc artists:\t\t{discLibrary.Where(d => !d.IsDeleted).Select(d => d.Artist).Where(a => a != null).Distinct().Count()}"));
				dbStatistics.AppendLine(Current($"\tDiscs totally:\t\t{discLibrary.Count(d => !d.IsDeleted)}"));
				dbStatistics.AppendLine(Current($"\tSongs totally:\t\t{discLibrary.Songs.Count(s => !s.IsDeleted)}"));
				Console.WriteLine(dbStatistics.ToString());

				foreach (var disc in discAdviser.AdviseDiscs(discLibrary).Take(AdvisedDiscsNumber))
				{
					Console.WriteLine(Current($"[{FormatPlaybackTime(disc.LastPlaybackTime)}]  {disc.Artist?.Name,-30} {disc.Title}"));
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
