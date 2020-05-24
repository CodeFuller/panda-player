using System;
using System.Globalization;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Dal.LocalDb.Extensions
{
	internal static class Int32Extensions
	{
		public static ItemId ToItemId(this Int32 id)
		{
			return new ItemId(id.ToString(CultureInfo.InvariantCulture));
		}
	}
}
