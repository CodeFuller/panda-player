using System;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Logic.Interfaces.Dal;
using MusicLibrary.Logic.Interfaces.Services;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Logic.Services
{
	internal class FoldersService : IFoldersService
	{
		private readonly IFoldersRepository foldersRepository;

		public FoldersService(IFoldersRepository foldersRepository)
		{
			this.foldersRepository = foldersRepository ?? throw new ArgumentNullException(nameof(foldersRepository));
		}

		public Task<FolderModel> GetRootFolder(CancellationToken cancellationToken)
		{
			return foldersRepository.GetRootFolder(cancellationToken);
		}

		public Task<FolderModel> GetFolder(ItemId folderId, CancellationToken cancellationToken)
		{
			return foldersRepository.GetFolder(folderId, cancellationToken);
		}

		public Task<FolderModel> GetDiscFolder(ItemId discId, CancellationToken cancellationToken)
		{
			return foldersRepository.GetDiscFolder(discId, cancellationToken);
		}
	}
}
