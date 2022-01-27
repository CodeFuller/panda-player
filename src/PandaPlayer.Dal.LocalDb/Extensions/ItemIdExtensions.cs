using System;
using System.Globalization;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Dal.LocalDb.Extensions
{
	internal static class ItemIdExtensions
	{
		public static int ToInt32(this ItemId id)
		{
			return Int32.Parse(id.Value, NumberStyles.None, CultureInfo.InvariantCulture);
		}

		public static ItemId ToItemId(this Int32 id)
		{
			return new(id.ToString(CultureInfo.InvariantCulture));
		}
	}
}
