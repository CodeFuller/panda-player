using System;
using System.Threading;
using System.Threading.Tasks;

namespace CF.MusicLibrary.LibraryToolkit.Interfaces
{
	public interface ISeedApiDatabaseCommand
	{
		Task Execute(string sourceDatabaseFileName, Uri apiBaseUri, CancellationToken cancellationToken);
	}
}
