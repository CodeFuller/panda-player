using System;
using System.Globalization;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Dal.LocalDb.Extensions
{
	public static class ItemIdExtensions
	{
		public static int ToInt32(this ItemId id)
		{
			return Int32.Parse(id.Value, NumberStyles.None, CultureInfo.InvariantCulture);
		}

		public static ItemId ToItemId(this Int32 id)
		{
			return new ItemId(id.ToString(CultureInfo.InvariantCulture));
		}
	}
}
