using System;
using MusicLibrary.Services.Diagnostic.Inconsistencies;

namespace MusicLibrary.Dal.LocalDb.Inconsistencies
{
	internal class IncorrectFileSizeInconsistency : LibraryInconsistency
	{
		private readonly string filePath;

		private readonly long expectedSize;

		private readonly long actualSize;

		public override string Description => $"The file size is incorrect: {actualSize} != {expectedSize} for '{filePath}'";

		public override InconsistencySeverity Severity => InconsistencySeverity.High;

		public IncorrectFileSizeInconsistency(string filePath, long expectedSize, long actualSize)
		{
			this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));

			this.expectedSize = expectedSize;
			this.actualSize = actualSize;
		}
	}
}
