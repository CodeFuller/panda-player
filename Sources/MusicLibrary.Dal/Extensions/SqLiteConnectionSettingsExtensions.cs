using System;
using Microsoft.EntityFrameworkCore;

namespace MusicLibrary.Dal.Extensions
{
	internal static class SqLiteConnectionSettingsExtensions
	{
		public static Action<DbContextOptionsBuilder> ToContextSetup(this SqLiteConnectionSettings settings)
		{
			var dataSourceFileName = settings.DataSource;

			if (String.IsNullOrEmpty(dataSourceFileName))
			{
				throw new InvalidOperationException("Database source file name is not defined");
			}

			var connectionString = dataSourceFileName.ToConnectionString();
			return dbContextOptionsBuilder => dbContextOptionsBuilder.UseSqlite(connectionString);
		}
	}
}
