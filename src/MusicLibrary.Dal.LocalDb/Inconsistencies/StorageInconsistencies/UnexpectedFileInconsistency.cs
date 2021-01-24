using System;
using MusicLibrary.Services.Diagnostic.Inconsistencies;

namespace MusicLibrary.Dal.LocalDb.Inconsistencies.StorageInconsistencies
{
	internal class UnexpectedFileInconsistency : LibraryInconsistency
	{
		private readonly string filePath;

		public override string Description => $"The file is unexpected: '{filePath}'";

		public override InconsistencySeverity Severity => InconsistencySeverity.Medium;

		public UnexpectedFileInconsistency(string filePath)
		{
			this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
		}
	}
}
