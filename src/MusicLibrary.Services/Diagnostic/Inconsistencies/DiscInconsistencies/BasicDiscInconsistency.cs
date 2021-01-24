using System;
using MusicLibrary.Core.Models;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.Services.Diagnostic.Inconsistencies.DiscInconsistencies
{
	internal abstract class BasicDiscInconsistency : LibraryInconsistency
	{
		protected DiscModel Disc { get; }

		protected string DiscDisplayTitle => Current($"{Disc.Folder.Name}/{Disc.TreeTitle}");

		protected BasicDiscInconsistency(DiscModel disc)
		{
			Disc = disc ?? throw new ArgumentNullException(nameof(disc));
		}
	}
}
