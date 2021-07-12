using System;
using PandaPlayer.Services.Diagnostic.Inconsistencies;

namespace PandaPlayer.Dal.LocalDb.Inconsistencies.StorageInconsistencies
{
	internal class MissingFolderInconsistency : LibraryInconsistency
	{
		private readonly string folderPath;

		public override string Description => $"The folder is missing: '{folderPath}'";

		public override InconsistencySeverity Severity => InconsistencySeverity.High;

		public MissingFolderInconsistency(string folderPath)
		{
			this.folderPath = folderPath ?? throw new ArgumentNullException(nameof(folderPath));
		}
	}
}
