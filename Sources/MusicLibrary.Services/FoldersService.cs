﻿using System;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;
using MusicLibrary.Services.Interfaces;
using MusicLibrary.Services.Interfaces.Dal;

namespace MusicLibrary.Services
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
			// TODO: Need to filter deleted discs
			return foldersRepository.GetRootFolder(cancellationToken);
		}

		public Task<FolderModel> GetFolder(ItemId folderId, CancellationToken cancellationToken)
		{
			// TODO: Need to filter deleted discs
			return foldersRepository.GetFolder(folderId, cancellationToken);
		}

		public Task<FolderModel> GetDiscFolder(ItemId discId, CancellationToken cancellationToken)
		{
			// TODO: Need to filter deleted discs
			return foldersRepository.GetDiscFolder(discId, cancellationToken);
		}
	}
}