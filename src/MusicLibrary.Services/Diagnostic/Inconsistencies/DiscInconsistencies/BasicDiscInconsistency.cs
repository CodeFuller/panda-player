using System;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.Diagnostic.Inconsistencies.DiscInconsistencies
{
	internal abstract class BasicDiscInconsistency : LibraryInconsistency
	{
		protected DiscModel Disc { get; }

		protected string DiscDisplayTitle => $"{Disc.Folder.Name}/{Disc.TreeTitle}";

		protected BasicDiscInconsistency(DiscModel disc)
		{
			Disc = disc ?? throw new ArgumentNullException(nameof(disc));
		}
	}
}
