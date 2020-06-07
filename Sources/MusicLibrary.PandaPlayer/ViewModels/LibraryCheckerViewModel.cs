using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using CF.Library.Wpf;
using GalaSoft.MvvmLight;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using MusicLibrary.Services.Diagnostic;
using MusicLibrary.Services.Diagnostic.Inconsistencies;
using MusicLibrary.Services.Interfaces;

namespace MusicLibrary.PandaPlayer.ViewModels
{
	internal class LibraryCheckerViewModel : ViewModelBase, ILibraryCheckerViewModel
	{
		private readonly IDiagnosticService diagnosticService;

		private bool checkDiscsConsistency = true;

		public bool CheckDiscsConsistency
		{
			get => checkDiscsConsistency;
			set => Set(ref checkDiscsConsistency, value);
		}

		private bool checkStorageConsistency = true;

		public bool CheckStorageConsistency
		{
			get => checkStorageConsistency;
			set => Set(ref checkStorageConsistency, value);
		}

		private bool checkContentConsistency = false;

		public bool CheckContentConsistency
		{
			get => checkContentConsistency;
			set => Set(ref checkContentConsistency, value);
		}

		private LibraryCheckFlags LibraryCheckFlags
		{
			get
			{
				var libraryCheckFlags = LibraryCheckFlags.None;
				if (CheckDiscsConsistency)
				{
					libraryCheckFlags |= LibraryCheckFlags.CheckDiscsConsistency;
				}

				if (CheckStorageConsistency)
				{
					libraryCheckFlags |= LibraryCheckFlags.CheckStorageConsistency;
				}

				if (CheckContentConsistency)
				{
					libraryCheckFlags |= LibraryCheckFlags.CheckContentConsistency;
				}

				return libraryCheckFlags;
			}
		}

		private bool isRunning;

		public bool IsRunning
		{
			get => isRunning;
			set => Set(ref isRunning, value);
		}

		public ICommand RunCheckCommand { get; }

		public ObservableCollection<DiagnosticInconsistencyViewModel> Inconsistencies { get; } = new ObservableCollection<DiagnosticInconsistencyViewModel>();

		public LibraryCheckerViewModel(IDiagnosticService diagnosticService)
		{
			this.diagnosticService = diagnosticService ?? throw new ArgumentNullException(nameof(diagnosticService));

			RunCheckCommand = new AsyncRelayCommand(() => RunCheck(CancellationToken.None));
		}

		private async Task RunCheck(CancellationToken cancellationToken)
		{
			IsRunning = true;
			Inconsistencies.Clear();

			await Task.Run(() => CheckLibrary(cancellationToken), cancellationToken);

			IsRunning = false;
		}

		private async Task CheckLibrary(CancellationToken cancellationToken)
		{
			void InconsistenciesHandler(LibraryInconsistency inconsistency)
			{
				Application.Current.Dispatcher.Invoke(() => Inconsistencies.Add(new DiagnosticInconsistencyViewModel(inconsistency)), DispatcherPriority.ContextIdle, cancellationToken);
			}

			await diagnosticService.CheckLibrary(LibraryCheckFlags, InconsistenciesHandler, cancellationToken);
		}
	}
}
