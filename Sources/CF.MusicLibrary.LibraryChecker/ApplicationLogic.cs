using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CF.Library.Bootstrap;
using CF.Library.Core.Exceptions;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.LibraryChecker.Checkers;
using Microsoft.Extensions.Logging;
using NDesk.Options;
using static System.FormattableString;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.LibraryChecker
{
	public class ApplicationLogic : IApplicationLogic
	{
		private readonly IDiscConsistencyChecker discConsistencyChecker;
		private readonly IStorageConsistencyChecker storageConsistencyChecker;
		private readonly ITagDataConsistencyChecker tagDataChecker;
		private readonly ILastFMConsistencyChecker lastFMConsistencyChecker;
		private readonly IDiscImagesConsistencyChecker discImagesConsistencyChecker;
		private readonly IMusicLibrary musicLibrary;
		private readonly ILogger<ApplicationLogic> logger;

		public ApplicationLogic(IDiscConsistencyChecker discConsistencyChecker, IStorageConsistencyChecker storageConsistencyChecker,
			ITagDataConsistencyChecker tagDataChecker, ILastFMConsistencyChecker lastFMConsistencyChecker,
			IDiscImagesConsistencyChecker discImagesConsistencyChecker, IMusicLibrary musicLibrary,
			ILogger<ApplicationLogic> logger)
		{
			this.discConsistencyChecker = discConsistencyChecker ?? throw new ArgumentNullException(nameof(discConsistencyChecker));
			this.storageConsistencyChecker = storageConsistencyChecker ?? throw new ArgumentNullException(nameof(storageConsistencyChecker));
			this.tagDataChecker = tagDataChecker ?? throw new ArgumentNullException(nameof(tagDataChecker));
			this.lastFMConsistencyChecker = lastFMConsistencyChecker ?? throw new ArgumentNullException(nameof(lastFMConsistencyChecker));
			this.discImagesConsistencyChecker = discImagesConsistencyChecker ?? throw new ArgumentNullException(nameof(discImagesConsistencyChecker));
			this.musicLibrary = musicLibrary ?? throw new ArgumentNullException(nameof(musicLibrary));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<int> Run(string[] args, CancellationToken cancellationToken)
		{
			LibraryCheckFlags checkFlags = LibraryCheckFlags.CheckDiscsConsistency | LibraryCheckFlags.CheckLibraryStorage;
			LaunchCommand command = LaunchCommand.ShowHelp;
			bool fixIssues = false;

			var options = new Dictionary<string, LibraryCheckFlags>
			{
				{ "check-discs", LibraryCheckFlags.CheckDiscsConsistency },
				{ "check-storage", LibraryCheckFlags.CheckLibraryStorage },
				{ "check-checksums", LibraryCheckFlags.CheckChecksums },
				{ "check-tags", LibraryCheckFlags.CheckTagData },
				{ "check-images", LibraryCheckFlags.CheckImages },
				{ "check-artists", LibraryCheckFlags.CheckArtistsOnLastFM },
				{ "check-albums", LibraryCheckFlags.CheckAlbumsOnLastFM },
				{ "check-songs", LibraryCheckFlags.CheckSongsOnLastFM },
			};
			var optionSet = new OptionSet();
			foreach (var option in options)
			{
				optionSet.Add(Invariant($"{option.Key}="), settingValue => checkFlags = UpdateCheckFlags(settingValue, checkFlags, option.Value));
			}

			optionSet.Add("check", s => command = LaunchCommand.Check);
			optionSet.Add("fix", s => fixIssues = true);
			optionSet.Add("unify-tags", s => command = LaunchCommand.UnifyTags);

			optionSet.Parse(args);

			switch (command)
			{
				case LaunchCommand.ShowHelp:
					ShowHelp();
					return 1;

				case LaunchCommand.Check:
					await RunChecks(checkFlags, fixIssues, cancellationToken);
					break;

				case LaunchCommand.UnifyTags:
					await UnifyTags(cancellationToken);
					break;

				default:
					throw new UnexpectedEnumValueException(command);
			}

			return 0;
		}

		private static void ShowHelp()
		{
			Console.Error.WriteLine();
			Console.Error.WriteLine("Usage: LibraryChecker.exe <command> [command options]");
			Console.Error.WriteLine("Supported commands:");
			Console.Error.WriteLine();
			Console.Error.WriteLine("  --check [--fix]  Check library consistency. Possible checks:");
			Console.Error.WriteLine();
			Console.Error.WriteLine("        --check-discs=yes|no        Default");
			Console.Error.WriteLine("        --check-storage=yes|no      Default");
			Console.Error.WriteLine("        --check-checksums=yes|no");
			Console.Error.WriteLine("        --check-tags=yes|no");
			Console.Error.WriteLine("        --check-images=yes|no");
			Console.Error.WriteLine("        --check-artists=yes|no");
			Console.Error.WriteLine("        --check-albums=yes|no");
			Console.Error.WriteLine("        --check-songs=yes|no");
			Console.Error.WriteLine();
			Console.Error.WriteLine("  --unify-tags     Rebuilds tags from the scratch");
		}

		private static bool ParseSetFlag(string setValue)
		{
			if (String.Equals(setValue, "yes", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}

			if (String.Equals(setValue, "no", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}

			throw new InvalidInputDataException(Current($"Could not parse setting value '{setValue}'"));
		}

		private static LibraryCheckFlags UpdateCheckFlags(string setValue, LibraryCheckFlags currlags, LibraryCheckFlags setFlag)
		{
			return ParseSetFlag(setValue) ? (currlags | setFlag) : (currlags & ~setFlag);
		}

		private async Task RunChecks(LibraryCheckFlags checkFlags, bool fixIssues, CancellationToken cancellationToken)
		{
			logger.LogInformation("Loading library content...");
			var discLibrary = await musicLibrary.LoadLibrary();

			if ((checkFlags & LibraryCheckFlags.CheckDiscsConsistency) != 0)
			{
				await discConsistencyChecker.CheckDiscsConsistency(discLibrary.Discs, cancellationToken);
			}

			if ((checkFlags & LibraryCheckFlags.CheckLibraryStorage) != 0)
			{
				await storageConsistencyChecker.CheckStorage(discLibrary, fixIssues, cancellationToken);
			}

			if ((checkFlags & LibraryCheckFlags.CheckChecksums) != 0)
			{
				await storageConsistencyChecker.CheckStorageChecksums(discLibrary, fixIssues, cancellationToken);
			}

			if ((checkFlags & LibraryCheckFlags.CheckTagData) != 0)
			{
				await tagDataChecker.CheckTagData(discLibrary.Songs, cancellationToken);
			}

			if ((checkFlags & LibraryCheckFlags.CheckImages) != 0)
			{
				await discImagesConsistencyChecker.CheckDiscImagesConsistency(discLibrary.Discs, cancellationToken);
			}

			if ((checkFlags & LibraryCheckFlags.CheckArtistsOnLastFM) != 0)
			{
				await lastFMConsistencyChecker.CheckArtists(discLibrary, cancellationToken);
			}

			if ((checkFlags & LibraryCheckFlags.CheckAlbumsOnLastFM) != 0)
			{
				await lastFMConsistencyChecker.CheckAlbums(discLibrary.Discs, cancellationToken);
			}

			if ((checkFlags & LibraryCheckFlags.CheckSongsOnLastFM) != 0)
			{
				await lastFMConsistencyChecker.CheckSongs(discLibrary.Songs, cancellationToken);
			}

			logger.LogInformation("Library check has finished");
		}

		private async Task UnifyTags(CancellationToken cancellationToken)
		{
			logger.LogInformation("Loading library content...");
			var discLibrary = await musicLibrary.LoadLibrary();

			await tagDataChecker.UnifyTags(discLibrary.Songs, cancellationToken);
		}
	}
}
