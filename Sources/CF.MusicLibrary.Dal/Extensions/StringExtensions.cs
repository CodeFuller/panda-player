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

				// ForeignKeys is available only since Microsoft.Data.Sqlite 3.0+
				// We can not user version 3.0+ of Microsoft.Data.Sqlite due to https://github.com/dotnet/efcore/issues/19396
				// TBD: Enable foreign keys constraint after switch to .NET Core.
				// ForeignKeys = true,
			};

			return builder.ConnectionString;
		}
	}
}
