using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.SQLite;
using System.Data.SQLite.EF6;

namespace CF.MusicLibrary.Dal
{
	// https://stackoverflow.com/questions/32319930/sqlite-entity-framework-without-app-config
	public class SqLiteDbConfiguration : DbConfiguration
	{
		public SqLiteDbConfiguration()
		{
			string assemblyName = typeof(SQLiteProviderFactory).Assembly.GetName().Name;

			SetProviderFactory(assemblyName, SQLiteFactory.Instance);
			SetProviderFactory(assemblyName, SQLiteProviderFactory.Instance);
			SetProviderServices(assemblyName, (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));
		}
	}
}
