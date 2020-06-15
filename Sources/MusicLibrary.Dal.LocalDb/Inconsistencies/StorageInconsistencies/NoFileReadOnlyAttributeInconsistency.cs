using System;
using MusicLibrary.Services.Diagnostic.Inconsistencies;

namespace MusicLibrary.Dal.LocalDb.Inconsistencies.StorageInconsistencies
{
	internal class NoFileReadOnlyAttributeInconsistency : LibraryInconsistency
	{
		private readonly string filePath;

		public override string Description => $"The file has no read-only attribute: '{filePath}'";

		public override InconsistencySeverity Severity => InconsistencySeverity.Medium;

		public NoFileReadOnlyAttributeInconsistency(string filePath)
		{
			this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
		}
	}
}
