using System;
using MusicLibrary.Services.Diagnostic.Inconsistencies;

namespace MusicLibrary.Dal.LocalDb.Inconsistencies
{
	internal class MissingFileInconsistency : LibraryInconsistency
	{
		private readonly string filePath;

		public override string Description => $"The file is missing: '{filePath}'";

		public override InconsistencySeverity Severity => InconsistencySeverity.High;

		public MissingFileInconsistency(string filePath)
		{
			this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
		}
	}
}
