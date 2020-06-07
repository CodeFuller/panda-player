using System;
using MusicLibrary.Services.Diagnostic.Inconsistencies;

namespace MusicLibrary.Dal.LocalDb.Inconsistencies
{
	internal class IncorrectFileChecksumInconsistency : LibraryInconsistency
	{
		private readonly string filePath;

		private readonly uint expectedChecksum;

		private readonly uint actualChecksum;

		public override string Description => $"The file checksum is incorrect: 0x{actualChecksum:X8} != 0x{expectedChecksum:X8} for '{filePath}'";

		public override InconsistencySeverity Severity => InconsistencySeverity.High;

		public IncorrectFileChecksumInconsistency(string filePath, uint expectedChecksum, uint actualChecksum)
		{
			this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));

			this.expectedChecksum = expectedChecksum;
			this.actualChecksum = actualChecksum;
		}
	}
}
