using System;
using GalaSoft.MvvmLight;
using PandaPlayer.Services.Diagnostic.Inconsistencies;

namespace PandaPlayer.ViewModels
{
	public class DiagnosticInconsistencyViewModel : ViewModelBase
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
