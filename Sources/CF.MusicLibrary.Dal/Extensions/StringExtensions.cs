using Microsoft.Data.Sqlite;

namespace CF.MusicLibrary.Dal.Extensions
{
	internal static class StringExtensions
	{
		public static string ToConnectionString(this string sqLiteFileName)
		{
			var builder = new SqliteConnectionStringBuilder
			{
				DataSource = sqLiteFileName,
				ForeignKeys = true,
			};

			return builder.ConnectionString;
		}
	}
}
