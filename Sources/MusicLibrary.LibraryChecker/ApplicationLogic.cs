using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CF.Library.Bootstrap;
using MusicLibrary.Core;
using MusicLibrary.Core.Interfaces;
using MusicLibrary.LibraryChecker.Checkers;
using NDesk.Options;

namespace MusicLibrary.LibraryChecker
{
	public class ApplicationLogic : IApplicationLogic
	{
		private readonly ILibraryConsistencyChecker libraryChecker;
		private readonly IUriCheckScope uriCheckScope;

		public ApplicationLogic(ILibraryConsistencyChecker libraryChecker, IUriCheckScope uriCheckScope)
		{
			this.libraryChecker = libraryChecker ?? throw new ArgumentNullException(nameof(libraryChecker));
			this.uriCheckScope = uriCheckScope ?? throw new ArgumentNullException(nameof(uriCheckScope));
		}

		public async Task<int> Run(string[] args, CancellationToken cancellationToken)
		{
			var command = LaunchCommandFlags.None;
			var checkFlags = LibraryCheckFlags.None;
			var scopeUri = ItemUriParts.RootUri;
			bool fixIssues = false;

			var options = new Dictionary<string, LibraryCheckFlags>
			{
				{ "check", LibraryCheckFlags.CheckDiscsConsistency | LibraryCheckFlags.CheckLibraryStorage },
				{ "check-discs", LibraryCheckFlags.CheckDiscsConsistency },
				{ "check-storage", LibraryCheckFlags.CheckLibraryStorage },
				{ "check-checksums", LibraryCheckFlags.CheckChecksums },
				{ "check-tags", LibraryCheckFlags.CheckTagData },
				{ "check-images", LibraryCheckFlags.CheckImages },
				{ "check-artists", LibraryCheckFlags.CheckArtistsOnLastFm },
				{ "check-albums", LibraryCheckFlags.CheckAlbumsOnLastFm },
				{ "check-songs", LibraryCheckFlags.CheckSongsOnLastFm },
			};
			var optionSet = new OptionSet();
			foreach (var option in options)
			{
				optionSet.Add(option.Key, settingValue =>
				{
					checkFlags |= option.Value;
					command |= LaunchCommandFlags.Check;
				});
			}

			optionSet.Add("scope=", s => scopeUri = new Uri(s, UriKind.Relative));
			optionSet.Add("fix", s => fixIssues = true);
			optionSet.Add("unify-tags", s => command |= LaunchCommandFlags.UnifyTags);

			optionSet.Parse(args);

			uriCheckScope.SetScopeUri(scopeUri);

			switch (command)
			{
				case LaunchCommandFlags.Check:
					await libraryChecker.CheckLibrary(checkFlags, fixIssues, cancellationToken);
					break;

				case LaunchCommandFlags.UnifyTags:
					await libraryChecker.UnifyTags(cancellationToken);
					break;

				default:
					ShowHelp();
					return 1;
			}

			return 0;
		}

		private static void ShowHelp()
		{
			Console.Error.WriteLine();
			Console.Error.WriteLine("Usage: LibraryChecker.exe <command> [--scope=\"/\"]");
			Console.Error.WriteLine("Supported commands:");
			Console.Error.WriteLine();
			Console.Error.WriteLine("  --check                     Shortcut for --check-discs --check-storage.");
			Console.Error.WriteLine("  --check-discs               Checks discs data (titles, songs order, ...).");
			Console.Error.WriteLine("  --check-storage   [--fix]   Checks storage data (files existence, unexpected files, ...).");
			Console.Error.WriteLine("  --check-checksums [--fix]   Checks storage data checksums.");
			Console.Error.WriteLine("  --check-tags                Checks tags data.");
			Console.Error.WriteLine("  --check-images              Checks disc images (sizes, formats, ...).");
			Console.Error.WriteLine("  --check-artists             Checks library artist names at Last.fm catalog.");
			Console.Error.WriteLine("  --check-albums              Checks library album titles at Last.fm catalog.");
			Console.Error.WriteLine("  --check-songs               Checks library song titles at Last.fm catalog.");
			Console.Error.WriteLine();
			Console.Error.WriteLine("  --unify-tags                Refills songs tag data from the scratch.");
		}
	}
}
