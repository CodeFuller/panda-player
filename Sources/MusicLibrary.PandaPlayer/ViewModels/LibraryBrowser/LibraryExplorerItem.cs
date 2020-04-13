using System;
using GalaSoft.MvvmLight;
using MusicLibrary.Core;

namespace MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser
{
	public abstract class LibraryExplorerItem : ViewModelBase
	{
		public virtual string Name { get; }

		public Uri Uri { get; }

		protected LibraryExplorerItem(Uri uri)
		{
			var uriParts = new ItemUriParts(uri);
			Name = uriParts.Count > 0 ? uriParts[uriParts.Count - 1] : String.Empty;
			Uri = uri;
		}
	}
}
