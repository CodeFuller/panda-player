using System;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Services.Diagnostic;
using MusicLibrary.Services.Diagnostic.Inconsistencies;

namespace MusicLibrary.Services.Interfaces
{
	public interface IDiagnosticService
	{
		Task CheckLibrary(LibraryCheckFlags checkFlags, Action<LibraryInconsistency> inconsistenciesHandler, CancellationToken cancellationToken);
	}
}
