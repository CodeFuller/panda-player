using System.Collections.Generic;

namespace PandaPlayer.Dal.LocalDb
{
	public class FileSystemStorageSettings
	{
		public string Root { get; set; }

		public IReadOnlyCollection<string> ExcludePaths { get; set; }
	}
}
