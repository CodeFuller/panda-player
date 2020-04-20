using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core;
using MusicLibrary.Core.Objects;
using MusicLibrary.Dal.Abstractions.Dto;
using MusicLibrary.Dal.Abstractions.Dto.Folders;
using MusicLibrary.Dal.Abstractions.Interfaces;
using MusicLibrary.Dal.LocalDb.Extensions;

namespace MusicLibrary.Dal.LocalDb
{
	// TBD: Remove after redesign
	internal class FolderReader : IFolderReader
	{
		private readonly DiscLibrary discLibrary;

		public FolderReader(DiscLibrary discLibrary)
		{
			this.discLibrary = discLibrary ?? throw new ArgumentNullException(nameof(discLibrary));
		}

		public Task<FolderData> GetRootFolder(bool includeDeletedDiscs, CancellationToken cancellationToken)
		{
			var rootId = new ItemId("/");
			return GetFolder(rootId, includeDeletedDiscs, cancellationToken);
		}

		public Task<FolderData> GetFolder(ItemId folderId, bool includeDeletedDiscs, CancellationToken cancellationToken)
		{
			var subfolders = new Dictionary<Uri, SubfolderData>();
			var discs = new List<FolderDiscData>();

			foreach (var disc in discLibrary.Discs)
			{
				var childUri = GetDirectChildUri(folderId, disc.Uri);
				if (childUri == null)
				{
					continue;
				}

				if (childUri == disc.Uri)
				{
					var discData = new FolderDiscData
					{
						Id = disc.Uri.ToItemId(),
						TreeTitle = new ItemUriParts(disc.Uri).Last(),
						Disc = disc,
					};

					discs.Add(discData);
				}
				else if (!subfolders.ContainsKey(childUri))
				{
					var subfolder = new SubfolderData
					{
						Id = childUri.ToItemId(),
						Name = new ItemUriParts(childUri).Last(),
					};

					subfolders.Add(childUri, subfolder);
				}
			}

			var uriParts = new ItemUriParts(folderId.ToUri());
			var parentFolderId = uriParts.Any() ? ItemUriParts.Join(uriParts.Take(uriParts.Count - 1)).ToItemId() : null;

			var folderData = new FolderData
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

		public Task<FolderData> GetDiscFolder(ItemId discId, CancellationToken cancellationToken)
		{
			var uriParts = new ItemUriParts(discId.ToUri());
			var parentFolderId = ItemUriParts.Join(uriParts.Take(uriParts.Count - 1)).ToItemId();

			return GetFolder(parentFolderId, false, CancellationToken.None);
		}
	}
}
