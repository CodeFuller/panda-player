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

		private ShallowFolderModel Folder { get; set; }

		public string FolderName { get; set; }

		public RenameFolderViewModel(IFoldersService foldersService)
		{
			this.foldersService = foldersService ?? throw new ArgumentNullException(nameof(foldersService));
		}

		public void Load(ShallowFolderModel folder)
		{
			Folder = folder;
			FolderName = folder.Name;
		}

		public Task Rename(CancellationToken cancellationToken)
		{
			Folder.Name = FolderName;

			return foldersService.UpdateFolder(Folder, cancellationToken);
		}
	}
}
