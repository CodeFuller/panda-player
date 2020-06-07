using System;
using GalaSoft.MvvmLight;
using MusicLibrary.Services.Diagnostic.Inconsistencies;

namespace MusicLibrary.PandaPlayer.ViewModels
{
	internal class DiagnosticInconsistencyViewModel : ViewModelBase
	{
		private readonly LibraryInconsistency inconsistency;

		public string Description => inconsistency.Description;

		public InconsistencySeverity Severity => inconsistency.Severity;

		public DiagnosticInconsistencyViewModel(LibraryInconsistency inconsistency)
		{
			this.inconsistency = inconsistency ?? throw new ArgumentNullException(nameof(inconsistency));
		}
	}
}
