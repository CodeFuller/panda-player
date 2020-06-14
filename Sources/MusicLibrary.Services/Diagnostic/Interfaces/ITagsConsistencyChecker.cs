using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;
using MusicLibrary.Services.Diagnostic.Inconsistencies;

namespace MusicLibrary.Services.Diagnostic.Interfaces
{
	internal interface ITagsConsistencyChecker
	{
		Task CheckTagsConsistency(IEnumerable<SongModel> songs, Action<LibraryInconsistency> inconsistenciesHandler, CancellationToken cancellationToken);
	}
}
