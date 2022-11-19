using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using CodeFuller.Library.Wpf;
using GalaSoft.MvvmLight;
using PandaPlayer.Services.Diagnostic;
using PandaPlayer.Services.Diagnostic.Inconsistencies;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels
{
	internal class LibraryCheckerViewModel : ViewModelBase, ILibraryCheckerViewModel, IOperationProgress
	{
		private readonly IDiagnosticService diagnosticService;

		public bool CheckDiscsConsistency
		{
			get => CheckFlags.HasFlag(LibraryCheckFlags.CheckDiscsConsistency);
			set
			{
				if (value)
				{
					CheckFlags |= LibraryCheckFlags.CheckDiscsConsistency;
				}
				else
				{
					CheckFlags &= ~LibraryCheckFlags.CheckDiscsConsistency;
				}
			}
		}

		public bool CheckStorageConsistency
		{
			get => CheckFlags.HasFlag(LibraryCheckFlags.CheckStorageConsistency);
			set
			{
				if (value)
				{
					CheckFlags |= LibraryCheckFlags.CheckStorageConsistency;
				}
				else
				{
					// If CheckStorage is off, then CheckContent and CheckTags are also off.
					CheckFlags &= ~LibraryCheckFlags.CheckContentConsistency;
					CheckFlags &= ~LibraryCheckFlags.CheckSongTagsConsistency;
				}
			}
		}

		public bool CheckContentConsistency
		{
			get => CheckFlags.HasFlag(LibraryCheckFlags.CheckContentConsistency);
			set
			{
				if (value)
				{
					CheckFlags |= LibraryCheckFlags.CheckContentConsistency;
				}
				else
				{
					// If CheckContent is off, CheckStorage is still on.
					CheckFlags &= ~LibraryCheckFlags.CheckContentConsistency;
					CheckFlags |= LibraryCheckFlags.CheckStorageConsistency;
				}
			}
		}

		public bool CheckTagsConsistency
		{
			get => CheckFlags.HasFlag(LibraryCheckFlags.CheckSongTagsConsistency);
			set
			{
				if (value)
				{
					CheckFlags |= LibraryCheckFlags.CheckSongTagsConsistency;
				}
				else
				{
					// If CheckTags is off, CheckStorage is still on.
					CheckFlags &= ~LibraryCheckFlags.CheckSongTagsConsistency;
					CheckFlags |= LibraryCheckFlags.CheckStorageConsistency;
				}
			}
		}

		private LibraryCheckFlags checkFlags = LibraryCheckFlags.CheckDiscsConsistency | LibraryCheckFlags.CheckStorageConsistency;

		private LibraryCheckFlags CheckFlags
		{
			get => checkFlags;
			set
			{
				checkFlags = value;

				RaisePropertyChanged(nameof(CheckDiscsConsistency));
				RaisePropertyChanged(nameof(CheckStorageConsistency));
				RaisePropertyChanged(nameof(CheckContentConsistency));
				RaisePropertyChanged(nameof(CheckTagsConsistency));
			}
		}

		private bool isRunning;

		public bool IsRunning
		{
			get => isRunning;
			set => Set(ref isRunning, value);
		}

		private double checkProgressMaximum = 100;

		public double CheckProgressMaximum
		{
			get => checkProgressMaximum;
			set
			{
				Set(ref checkProgressMaximum, value);
				RaisePropertyChanged(nameof(CheckProgressPercentage));
			}
		}

		private double checkProgressValue;

		public double CheckProgressValue
		{
			get => checkProgressValue;
			set
			{
				Set(ref checkProgressValue, value);
				RaisePropertyChanged(nameof(CheckProgressPercentage));
			}
		}

		public string CheckProgressPercentage => $"{(CheckProgressMaximum > 0 ? CheckProgressValue / CheckProgressMaximum * 100 : 0):N1}%";

		public ICommand RunCheckCommand { get; }

		public ObservableCollection<DiagnosticInconsistencyViewModel> Inconsistencies { get; } = new();

		public LibraryCheckerViewModel(IDiagnosticService diagnosticService)
		{
			this.diagnosticService = diagnosticService ?? throw new ArgumentNullException(nameof(diagnosticService));

			RunCheckCommand = new AsyncRelayCommand(() => RunCheck(CancellationToken.None));
		}

		private async Task RunCheck(CancellationToken cancellationToken)
		{
			IsRunning = true;
			CheckProgressValue = 0;
			Inconsistencies.Clear();

			await Task.Run(() => CheckLibrary(cancellationToken), cancellationToken);

			CheckProgressValue = CheckProgressMaximum;
			IsRunning = false;
		}

		private async Task CheckLibrary(CancellationToken cancellationToken)
		{
			void InconsistenciesHandler(LibraryInconsistency inconsistency)
			{
				Application.Current.Dispatcher.Invoke(() => Inconsistencies.Add(new DiagnosticInconsistencyViewModel(inconsistency)), DispatcherPriority.ContextIdle, cancellationToken);
			}

			await diagnosticService.CheckLibrary(CheckFlags, this, InconsistenciesHandler, cancellationToken);
		}

		public void SetOperationCost(int cost)
		{
			Application.Current.Dispatcher.Invoke(() => CheckProgressMaximum = cost);
		}

		public void IncrementOperationProgress()
		{
			Application.Current.Dispatcher.Invoke(() => CheckProgressValue++);
		}
	}
}
