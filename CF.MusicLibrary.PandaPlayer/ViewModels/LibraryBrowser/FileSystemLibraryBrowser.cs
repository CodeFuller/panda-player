using System;
using System.Collections.Generic;
using System.Linq;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser
{
	public class FileSystemLibraryBrowser : ILibraryBrowser
	{
		private readonly DiscLibrary discLibrary;

		public FileSystemLibraryBrowser(DiscLibrary discLibrary)
		{
			if (discLibrary == null)
			{
				throw new ArgumentNullException(nameof(discLibrary));
			}

			this.discLibrary = discLibrary;
		}

		public IEnumerable<FolderExplorerItem> GetChildFolderItems(FolderExplorerItem folderItem)
		{
			if (folderItem == null)
			{
				throw new ArgumentNullException(nameof(folderItem));
			}

			Dictionary<Uri, FolderExplorerItem> knownFolderUris = new Dictionary<Uri, FolderExplorerItem>();
			foreach (var disc in discLibrary)
			{
				var childUri = GetDirectChildUri(folderItem.Uri, disc.Uri);
				if (childUri != null && !knownFolderUris.ContainsKey(childUri))
				{
					FolderExplorerItem item = childUri == disc.Uri ? new DiscExplorerItem(disc) : new FolderExplorerItem(childUri);
					knownFolderUris.Add(childUri, item);
				}
			}

			return knownFolderUris.Values.Select(v => v);
		}

		private static Uri GetDirectChildUri(Uri parentUri, Uri childUri)
		{
			ItemUriParts parentParts = new ItemUriParts(parentUri);
			ItemUriParts childParts = new ItemUriParts(childUri);

			return parentParts.IsBaseOf(childParts) ? ItemUriParts.Join(childParts.Take(parentParts.Count + 1)) : null;
		}

		public FolderExplorerItem GetParentFolder(FolderExplorerItem folderItem)
		{
			if (folderItem == null)
			{
				throw new ArgumentNullException(nameof(folderItem));
			}

			ItemUriParts childParts = new ItemUriParts(folderItem.Uri);
			if (childParts.Count == 0)
			{
				return null;
			}

			Uri parentUri = ItemUriParts.Join(childParts.Take(childParts.Count - 1));
			return new FolderExplorerItem(parentUri);
		}
	}
}
