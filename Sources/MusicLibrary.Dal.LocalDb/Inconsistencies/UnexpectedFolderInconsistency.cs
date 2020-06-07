using System;
using MusicLibrary.Services.Diagnostic.Inconsistencies;

namespace MusicLibrary.Dal.LocalDb.Inconsistencies
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
