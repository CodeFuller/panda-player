using System;
using PandaPlayer.Services.Diagnostic.Inconsistencies;

namespace PandaPlayer.Dal.LocalDb.Inconsistencies.StorageInconsistencies
{
	internal class UnexpectedFolderInconsistency : LibraryInconsistency
	{
		private readonly string folderPath;

		public override string Description => $"The folder is unexpected: '{folderPath}'";

		public override InconsistencySeverity Severity => InconsistencySeverity.Medium;

		public UnexpectedFolderInconsistency(string filePath)
		{
			this.folderPath = filePath ?? throw new ArgumentNullException(nameof(filePath));
		}
	}
}
