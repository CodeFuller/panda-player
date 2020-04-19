using System;
using System.Collections.Generic;
using System.Linq;
using MusicLibrary.Core;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser
{
	public class FileSystemLibraryBrowser : ILibraryBrowser
	{
		private readonly DiscLibrary discLibrary;

		public FileSystemLibraryBrowser(DiscLibrary discLibrary)
		{
			this.discLibrary = discLibrary ?? throw new ArgumentNullException(nameof(discLibrary));
		}

		public IEnumerable<LibraryExplorerItem> GetChildFolderItems(FolderExplorerItem folderItem)
		{
			_ = folderItem ?? throw new ArgumentNullException(nameof(folderItem));

			var knownFolderUris = new Dictionary<Uri, LibraryExplorerItem>();
			foreach (var disc in discLibrary.Discs)
			{
				var childUri = GetDirectChildUri(folderItem.Uri, disc.Uri);
				if (childUri != null && !knownFolderUris.ContainsKey(childUri))
				{
					var item = childUri == disc.Uri ? new DiscExplorerItem(disc) as LibraryExplorerItem : new FolderExplorerItem(childUri);
					knownFolderUris.Add(childUri, item);
				}
			}

			return knownFolderUris.Values.Select(v => v);
		}

		private static Uri GetDirectChildUri(Uri parentUri, Uri childUri)
		{
			var parentParts = new ItemUriParts(parentUri);
			var childParts = new ItemUriParts(childUri);

			return parentParts.IsBaseOf(childParts) ? ItemUriParts.Join(childParts.Take(parentParts.Count + 1)) : null;
		}

		public FolderExplorerItem GetParentFolder(LibraryExplorerItem item)
		{
			_ = item ?? throw new ArgumentNullException(nameof(item));

			var childParts = new ItemUriParts(item.Uri);
			if (childParts.Count == 0)
			{
				return null;
			}

			var parentUri = ItemUriParts.Join(childParts.Take(childParts.Count - 1));
			return new FolderExplorerItem(parentUri);
		}

		public DiscExplorerItem GetDiscItem(Disc disc)
		{
			return new DiscExplorerItem(disc);
		}
	}
}
