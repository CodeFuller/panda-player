using System;
using System.Threading;
using System.Threading.Tasks;
using CF.MusicLibrary.LibraryToolkit.Interfaces;

namespace CF.MusicLibrary.LibraryToolkit
{
	public class SeedApiDatabaseCommand : ISeedApiDatabaseCommand
	{
		public Task Execute(string sourceDatabaseFileName, Uri apiBaseUri, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
