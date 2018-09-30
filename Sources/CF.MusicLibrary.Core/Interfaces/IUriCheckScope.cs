using System;

namespace CF.MusicLibrary.Core.Interfaces
{
	public interface IUriCheckScope : ICheckScope
	{
		bool Contains(Uri uri);

		void SetScopeUri(Uri uri);
	}
}
