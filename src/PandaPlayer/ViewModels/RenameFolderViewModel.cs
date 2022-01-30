using System;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels
{
	internal class RenameFolderViewModel : IRenameFolderViewModel
	{
		private readonly IFoldersService foldersService;

		private FolderModel Folder { get; set; }

		public string FolderName { get; set; }

		public RenameFolderViewModel(IFoldersService foldersService)
		{
			this.foldersService = foldersService ?? throw new ArgumentNullException(nameof(foldersService));
		}

		public void Load(FolderModel folder)
		{
			Folder = folder;
			FolderName = folder.Name;
		}

		public Task Rename(CancellationToken cancellationToken)
		{
			void UpdateFolder(FolderModel folder)
			{
				folder.Name = FolderName;
			}

			return foldersService.UpdateFolder(Folder, UpdateFolder, cancellationToken);
		}
	}
}
