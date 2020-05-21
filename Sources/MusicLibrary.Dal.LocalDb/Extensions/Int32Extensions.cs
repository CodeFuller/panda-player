using System;
using System.Globalization;
using MusicLibrary.Logic.Models;

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
