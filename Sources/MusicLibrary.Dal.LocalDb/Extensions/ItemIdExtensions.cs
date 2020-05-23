using System;
using System.Globalization;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Dal.LocalDb.Extensions
{
	public static class ItemIdExtensions
	{
		public static Uri ToUri(this ItemId id)
		{
			return new Uri(id.Value, UriKind.Relative);
		}

		public static int ToInt32(this ItemId id)
		{
			return Int32.Parse(id.Value, NumberStyles.None, CultureInfo.InvariantCulture);
		}
	}
}
