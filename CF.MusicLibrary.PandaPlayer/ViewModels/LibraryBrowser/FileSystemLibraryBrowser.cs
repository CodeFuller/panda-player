using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.MyLocalLibrary;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser
{
	public class FileSystemLibraryBrowser : ILibraryBrowser
	{
		private const char UriDelimiter = '/';

		private readonly IMusicCatalog musicCatalog;
		private DiscLibrary discLibrary;

		public FileSystemLibraryBrowser(IMusicCatalog musicCatalog)
		{
			if (musicCatalog == null)
			{
				throw new ArgumentNullException(nameof(musicCatalog));
			}

			this.musicCatalog = musicCatalog;
		}

		public async Task Load()
		{
			discLibrary = await musicCatalog.GetDiscsAsync();
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
