using System;
using CommunityToolkit.Mvvm.ComponentModel;
using PandaPlayer.Services.Diagnostic.Inconsistencies;

namespace PandaPlayer.ViewModels
{
	public class DiagnosticInconsistencyViewModel : ObservableObject
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
