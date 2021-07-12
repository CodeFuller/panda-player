using System;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Services.Diagnostic;
using PandaPlayer.Services.Diagnostic.Inconsistencies;

namespace PandaPlayer.Services.Interfaces
{
	public interface IDiagnosticService
	{
		Task CheckLibrary(LibraryCheckFlags checkFlags, IOperationProgress progress, Action<LibraryInconsistency> inconsistenciesHandler, CancellationToken cancellationToken);
	}
}
