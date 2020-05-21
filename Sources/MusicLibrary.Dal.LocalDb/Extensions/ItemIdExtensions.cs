using System;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Dal.LocalDb.Extensions
{
	public static class ItemIdExtensions
	{
		public static Uri ToUri(this ItemId id)
		{
			return new Uri(id.Value, UriKind.Relative);
		}
	}
}
