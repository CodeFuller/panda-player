using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;
using PandaPlayer.DiscAdder.Interfaces;
using PandaPlayer.Services.Interfaces;

namespace PandaPlayer.DiscAdder.Internal
{
	internal class FolderProvider : IFolderProvider
	{
		private readonly IFoldersService foldersService;

		public FolderProvider(IFoldersService foldersService)
		{
			this.foldersService = foldersService ?? throw new ArgumentNullException(nameof(foldersService));
		}

		public async Task<FolderModel> GetFolder(IEnumerable<string> path, CancellationToken cancellationToken)
		{
			var currentFolder = await foldersService.GetRootFolder(cancellationToken);

			foreach (var currentSubfolderName in path)
			{
				var currentSubfolder = currentFolder.Subfolders.SingleOrDefault(sf => String.Equals(sf.Name, currentSubfolderName, StringComparison.Ordinal));
				if (currentSubfolder == null)
				{
					// The requested folder (or some of its parents) does not exist.
					return null;
				}

				currentFolder = await foldersService.GetFolder(currentSubfolder.Id, cancellationToken);
			}

			return currentFolder;
		}
	}
}
