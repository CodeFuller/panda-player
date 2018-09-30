using System.Data.Common;

namespace CF.MusicLibrary.Dal
{
	public interface IConfiguredDbConnectionFactory
	{
		DbConnection CreateConnection();
	}
}
