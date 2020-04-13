using System;
using MusicLibrary.Core;
using MusicLibrary.Core.Interfaces;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.LibraryChecker
{
	public class CheckScope : IUriCheckScope
	{
		private ItemUriParts scope;

		public CheckScope()
		{
			SetScopeUri(ItemUriParts.RootUri);
		}

		public bool Contains(Disc disc)
		{
			return Contains(disc.Uri);
		}

		public bool Contains(Song song)
		{
			return Contains(song.Uri);
		}

		public bool Contains(Uri uri)
		{
			return scope.IsBaseOf(new ItemUriParts(uri));
		}

		public void SetScopeUri(Uri uri)
		{
			scope = new ItemUriParts(uri);
		}
	}
}
