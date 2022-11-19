using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Diagnostic.Inconsistencies;

namespace PandaPlayer.Services.Diagnostic.Interfaces
{
	internal interface IDiscConsistencyChecker
	{
		Task CheckDiscsConsistency(IEnumerable<DiscModel> discs, Action<LibraryInconsistency> inconsistenciesHandler, CancellationToken cancellationToken);
	}
}
