using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core;
using MusicLibrary.Core.Objects;
using MusicLibrary.Dal.LocalDb.Extensions;
using MusicLibrary.Logic.Interfaces.Dal;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Dal.LocalDb
{
	// TBD: Remove after redesign
	internal class FoldersRepository : IFoldersRepository
	{
		private readonly DiscLibrary discLibrary;

		public FoldersRepository(DiscLibrary discLibrary)
		{
			this.discLibrary = discLibrary ?? throw new ArgumentNullException(nameof(discLibrary));
		}

		public Task<FolderModel> GetRootFolder(bool includeDeletedDiscs, CancellationToken cancellationToken)
		{
			var rootId = new ItemId("/");
			return GetFolder(rootId, includeDeletedDiscs, cancellationToken);
		}

		public Task<FolderModel> GetFolder(ItemId folderId, bool includeDeletedDiscs, CancellationToken cancellationToken)
		{
			var subfolders = new Dictionary<Uri, SubfolderModel>();
			var discs = new List<FolderDiscModel>();

			foreach (var disc in discLibrary.Discs)
			{
				var childUri = GetDirectChildUri(folderId, disc.Uri);
				if (childUri == null)
				{
					continue;
				}

				if (childUri == disc.Uri)
				{
					var discData = new FolderDiscModel
					{
						Id = disc.Id.ToItemId(),
						TreeTitle = new ItemUriParts(disc.Uri).Last(),
						Disc = disc.ToModel(),
					};

					discs.Add(discData);
				}
				else if (!subfolders.ContainsKey(childUri))
				{
					var subfolder = new SubfolderModel
					{
						Id = childUri.ToItemId(),
						Name = new ItemUriParts(childUri).Last(),
					};

					subfolders.Add(childUri, subfolder);
				}
			}

			var uriParts = new ItemUriParts(folderId.ToUri());
			var parentFolderId = uriParts.Any() ? ItemUriParts.Join(uriParts.Take(uriParts.Count - 1)).ToItemId() : null;

			var folderData = new FolderModel
			{
				Id = folderId,
				ParentFolderId = parentFolderId,
				Subfolders = subfolders.Values,
				Discs = discs.ToList(),
			};

			return Task.FromResult(folderData);
		}

		private static Uri GetDirectChildUri(ItemId folderId, Uri childUri)
		{
			var parentParts = new ItemUriParts(folderId.ToUri());
			var childParts = new ItemUriParts(childUri);

			return parentParts.IsBaseOf(childParts) ? ItemUriParts.Join(childParts.Take(parentParts.Count + 1)) : null;
		}

		public Task<FolderModel> GetDiscFolder(ItemId discId, CancellationToken cancellationToken)
		{
			var disc = discLibrary.Discs.Single(d => d.Id.ToItemId() == discId);

			var uriParts = new ItemUriParts(disc.Uri);
			var parentFolderId = ItemUriParts.Join(uriParts.Take(uriParts.Count - 1)).ToItemId();

			return GetFolder(parentFolderId, false, CancellationToken.None);
		}
	}
}
