using System;
using MusicLibrary.Services.Diagnostic.Inconsistencies;

namespace MusicLibrary.Dal.LocalDb.Inconsistencies
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
