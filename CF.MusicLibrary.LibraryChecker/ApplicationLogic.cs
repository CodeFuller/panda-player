using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CF.Library.Core.Bootstrap;
using CF.Library.Core.Exceptions;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.LibraryChecker.Checkers;
using NDesk.Options;
using static CF.Library.Core.Application;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.LibraryChecker
{
	public class ApplicationLogic : IApplicationLogic
	{
		private readonly IDiscConsistencyChecker discConsistencyChecker;
		private readonly ITagDataConsistencyChecker tagDataChecker;
		private readonly ILastFMConsistencyChecker lastFmConsistencyChecker;
		private readonly IMusicLibraryRepository libraryRepository;

		public ApplicationLogic(IDiscConsistencyChecker discConsistencyChecker, ITagDataConsistencyChecker tagDataChecker,
			ILastFMConsistencyChecker lastFMConsistencyChecker, IMusicLibraryRepository libraryRepository)
		{
			if (discConsistencyChecker == null)
			{
				throw new ArgumentNullException(nameof(discConsistencyChecker));
			}
			if (tagDataChecker == null)
			{
				throw new ArgumentNullException(nameof(tagDataChecker));
			}
			if (lastFMConsistencyChecker == null)
			{
				throw new ArgumentNullException(nameof(lastFMConsistencyChecker));
			}
			if (libraryRepository == null)
			{
				throw new ArgumentNullException(nameof(libraryRepository));
			}

			this.discConsistencyChecker = discConsistencyChecker;
			this.tagDataChecker = tagDataChecker;
			this.lastFmConsistencyChecker = lastFMConsistencyChecker;
			this.libraryRepository = libraryRepository;
		}

		public int Run(string[] args)
		{
			LibraryCheckFlags checkFlags = LibraryCheckFlags.CheckDiscsConsistency | LibraryCheckFlags.CheckTagData;
			LaunchCommand command = LaunchCommand.ShowHelp;

			var options = new Dictionary<string, LibraryCheckFlags>
			{
				{ "check-discs", LibraryCheckFlags.CheckDiscsConsistency },
				{ "check-tags", LibraryCheckFlags.CheckTagData },
				{ "check-artists", LibraryCheckFlags.CheckArtistsOnLastFM },
				{ "check-albums", LibraryCheckFlags.CheckAlbumsOnLastFM },
				{ "check-songs", LibraryCheckFlags.CheckSongsOnLastFM },
			};
			var optionSet = new OptionSet();
			foreach (var option in options)
			{
				optionSet.Add($"{option.Key}=", settingValue => checkFlags = UpdateCheckFlags(settingValue, checkFlags, option.Value));
			}

			optionSet.Add("check", s => command = LaunchCommand.Check);
			optionSet.Add("unify-tags", s => command = LaunchCommand.UnifyTags);

			optionSet.Parse(args);

			switch (command)
			{
				case LaunchCommand.ShowHelp:
					ShowHelp(optionSet);
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

		private void ShowHelp(OptionSet optionSet)
		{
			Console.Error.WriteLine();
			Console.Error.WriteLine("Usage: LibraryChecker.exe <command> [command options]");
			Console.Error.WriteLine("Supported commands:");
			Console.Error.WriteLine();
			Console.Error.WriteLine("  --check          Check library consistency. Possible checks");
			Console.Error.WriteLine();
			Console.Error.WriteLine("        --check-discs=yes|no        Default");
			Console.Error.WriteLine("        --check-tags=yes|no         Default");
			Console.Error.WriteLine("        --check-artists=yes|no");
			Console.Error.WriteLine("        --check-albums=yes|no");
			Console.Error.WriteLine("        --check-songs=yes|no");
			Console.Error.WriteLine();
			Console.Error.WriteLine("  --unify-tags     Rebuilds tags from the scratch");
		}

		private bool ParseSetFlag(string setValue)
		{
			if (String.Equals(setValue, "yes", StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}

			if (String.Equals(setValue, "no", StringComparison.InvariantCultureIgnoreCase))
			{
				return false;
			}

			throw new InvalidInputDataException(Current($"Could not parse setting value '{setValue}'"));
		}

		private LibraryCheckFlags UpdateCheckFlags(string setValue, LibraryCheckFlags currlags, LibraryCheckFlags setFlag)
		{
			if (ParseSetFlag(setValue))
			{
				return currlags | setFlag;
			}
			else
			{
				return currlags & ~setFlag;
			}
		}

		private async Task RunChecks(LibraryCheckFlags checkFlags)
		{
			Logger.WriteInfo("Loading library content...");

			var artists = (await libraryRepository.GetArtistsAsync()).OrderBy(artist => artist.Name);
			var discs = (await libraryRepository.GetDiscsAsync()).OrderBy(disc => disc.Uri.ToString());
			var songs = (await libraryRepository.GetSongsAsync()).OrderBy(song => song.Uri.ToString());

			if ((checkFlags & LibraryCheckFlags.CheckDiscsConsistency) != 0)
			{
				discConsistencyChecker.CheckDiscsConsistency(discs);
			}

			if ((checkFlags & LibraryCheckFlags.CheckTagData) != 0)
			{
				tagDataChecker.CheckTagData(songs);
			}

			if ((checkFlags & LibraryCheckFlags.CheckArtistsOnLastFM) != 0)
			{
				await lastFmConsistencyChecker.CheckArtists(artists);
			}

			if ((checkFlags & LibraryCheckFlags.CheckAlbumsOnLastFM) != 0)
			{
				await lastFmConsistencyChecker.CheckAlbums(discs);
			}

			if ((checkFlags & LibraryCheckFlags.CheckSongsOnLastFM) != 0)
			{
				await lastFmConsistencyChecker.CheckSongs(songs);
			}

			Logger.WriteInfo("Library check has finished");
		}

		private async Task UnifyTags()
		{
			var songs = (await libraryRepository.GetSongsAsync()).OrderBy(song => song.Uri.ToString());

			tagDataChecker.UnifyTags(songs);
		}
	}
}
