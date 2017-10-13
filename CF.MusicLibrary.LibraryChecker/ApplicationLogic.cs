using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CF.Library.Core.Bootstrap;
using CF.Library.Core.Exceptions;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.LibraryChecker.Checkers;
using NDesk.Options;
using static System.FormattableString;
using static CF.Library.Core.Application;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.LibraryChecker
{
	public class ApplicationLogic : IApplicationLogic
	{
		private readonly IDiscConsistencyChecker discConsistencyChecker;
		private readonly IStorageConsistencyChecker storageConsistencyChecker;
		private readonly ITagDataConsistencyChecker tagDataChecker;
		private readonly ILastFMConsistencyChecker lastFMConsistencyChecker;
		private readonly IDiscArtConsistencyChecker discArtConsistencyChecker;
		private readonly IMusicLibrary musicLibrary;

		public ApplicationLogic(IDiscConsistencyChecker discConsistencyChecker, IStorageConsistencyChecker storageConsistencyChecker,
			ITagDataConsistencyChecker tagDataChecker, ILastFMConsistencyChecker lastFMConsistencyChecker, IDiscArtConsistencyChecker discArtConsistencyChecker,
			IMusicLibrary musicLibrary)
		{
			if (discConsistencyChecker == null)
			{
				throw new ArgumentNullException(nameof(discConsistencyChecker));
			}
			if (storageConsistencyChecker == null)
			{
				throw new ArgumentNullException(nameof(storageConsistencyChecker));
			}
			if (tagDataChecker == null)
			{
				throw new ArgumentNullException(nameof(tagDataChecker));
			}
			if (lastFMConsistencyChecker == null)
			{
				throw new ArgumentNullException(nameof(lastFMConsistencyChecker));
			}
			if (discArtConsistencyChecker == null)
			{
				throw new ArgumentNullException(nameof(discArtConsistencyChecker));
			}
			if (musicLibrary == null)
			{
				throw new ArgumentNullException(nameof(musicLibrary));
			}

			this.discConsistencyChecker = discConsistencyChecker;
			this.storageConsistencyChecker = storageConsistencyChecker;
			this.tagDataChecker = tagDataChecker;
			this.lastFMConsistencyChecker = lastFMConsistencyChecker;
			this.discArtConsistencyChecker = discArtConsistencyChecker;
			this.musicLibrary = musicLibrary;
		}

		public int Run(string[] args)
		{
			LibraryCheckFlags checkFlags = LibraryCheckFlags.CheckDiscsConsistency | LibraryCheckFlags.CheckLibraryStorage;
			LaunchCommand command = LaunchCommand.ShowHelp;

			var options = new Dictionary<string, LibraryCheckFlags>
			{
				{ "check-discs", LibraryCheckFlags.CheckDiscsConsistency },
				{ "check-storage", LibraryCheckFlags.CheckLibraryStorage },
				{ "check-tags", LibraryCheckFlags.CheckTagData },
				{ "check-arts", LibraryCheckFlags.CheckDiscArts },
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
			optionSet.Add("unify-tags", s => command = LaunchCommand.UnifyTags);

			optionSet.Parse(args);

			switch (command)
			{
				case LaunchCommand.ShowHelp:
					ShowHelp();
					break;

				case LaunchCommand.Check:
					RunChecks(checkFlags).Wait();
					break;

				case LaunchCommand.UnifyTags:
					UnifyTags().Wait();
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
			Console.Error.WriteLine("  --check          Check library consistency. Possible checks");
			Console.Error.WriteLine();
			Console.Error.WriteLine("        --check-discs=yes|no        Default");
			Console.Error.WriteLine("        --check-storage=yes|no      Default");
			Console.Error.WriteLine("        --check-tags=yes|no");
			Console.Error.WriteLine("        --check-arts=yes|no");
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

		private async Task RunChecks(LibraryCheckFlags checkFlags)
		{
			Logger.WriteInfo("Loading library content...");
			var discLibrary = await musicLibrary.LoadLibrary();

			if ((checkFlags & LibraryCheckFlags.CheckDiscsConsistency) != 0)
			{
				await discConsistencyChecker.CheckDiscsConsistency(discLibrary.Discs);
			}

			if ((checkFlags & LibraryCheckFlags.CheckLibraryStorage) != 0)
			{
				await storageConsistencyChecker.CheckStorage(discLibrary);
			}

			if ((checkFlags & LibraryCheckFlags.CheckTagData) != 0)
			{
				await tagDataChecker.CheckTagData(discLibrary.Songs);
			}

			if ((checkFlags & LibraryCheckFlags.CheckDiscArts) != 0)
			{
				await discArtConsistencyChecker.CheckDiscArtsConsistency(discLibrary.Discs);
			}

			if ((checkFlags & LibraryCheckFlags.CheckArtistsOnLastFM) != 0)
			{
				await lastFMConsistencyChecker.CheckArtists(discLibrary);
			}

			if ((checkFlags & LibraryCheckFlags.CheckAlbumsOnLastFM) != 0)
			{
				await lastFMConsistencyChecker.CheckAlbums(discLibrary.Discs);
			}

			if ((checkFlags & LibraryCheckFlags.CheckSongsOnLastFM) != 0)
			{
				await lastFMConsistencyChecker.CheckSongs(discLibrary.Songs);
			}

			Logger.WriteInfo("Library check has finished");
		}

		private async Task UnifyTags()
		{
			Logger.WriteInfo("Loading library content...");
			var discLibrary = await musicLibrary.LoadLibrary();

			await tagDataChecker.UnifyTags(discLibrary.Songs);
		}
	}
}
