using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CodeFuller.Library.Wpf;
using CodeFuller.Library.Wpf.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PandaPlayer.Core.Comparers;
using PandaPlayer.Core.Extensions;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Shared.Extensions;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels.AdviseSetsEditor
{
	internal class AdviseSetsEditorViewModel : ViewModelBase, IAdviseSetsEditorViewModel
	{
		private readonly IAdviseSetService adviseSetService;

		private readonly IDiscsService discService;

		private readonly IWindowService windowService;

		public ObservableCollection<AdviseSetModel> AdviseSets { get; } = new();

		private AdviseSetModel selectedAdviseSet;

		public AdviseSetModel SelectedAdviseSet
		{
			get => selectedAdviseSet;
			set
			{
				Set(ref selectedAdviseSet, value);

				CurrentAdviseSetDiscs.Clear();
				if (selectedAdviseSet != null)
				{
					CurrentAdviseSetDiscs.AddRange(ActiveDiscs.Where(x => x.AdviseSetInfo?.AdviseSet.Id == selectedAdviseSet.Id));
				}

				AvailableDiscsViewModel.LoadAvailableDiscsForAdviseSet(CurrentAdviseSetDiscs);

				RaisePropertyChangedForAdviseSetButtons();
			}
		}

		public bool CanCreateAdviseSet => !AvailableDiscsViewModel.SelectedDiscs.Any() || AvailableDiscsViewModel.SelectedDiscsCanBeAddedToAdviseSet(Array.Empty<DiscModel>());

		public bool CanDeleteAdviseSet => SelectedAdviseSet != null;

		public ObservableCollection<DiscModel> CurrentAdviseSetDiscs { get; } = new();

		private DiscModel selectedAdviseSetDisc;

		public DiscModel SelectedAdviseSetDisc
		{
			get => selectedAdviseSetDisc;
			set
			{
				Set(ref selectedAdviseSetDisc, value);
				RaisePropertyChangedForAdviseSetButtons();
			}
		}

		private int SelectedAdviseSetDiscIndex => CurrentAdviseSetDiscs.Select((x, i) => new { Disc = x, Index = i }).FirstOrDefault(x => x.Disc.Id == SelectedAdviseSetDisc.Id)?.Index ?? -1;

		public bool CanAddDiscs => SelectedAdviseSet != null && AvailableDiscsViewModel.SelectedDiscsCanBeAddedToAdviseSet(CurrentAdviseSetDiscs);

		public bool CanRemoveDisc => SelectedAdviseSet != null && SelectedAdviseSetDisc != null;

		public bool CanMoveDiscUp => SelectedAdviseSet != null && SelectedAdviseSetDisc != null && SelectedAdviseSetDiscIndex != 0;

		public bool CanMoveDiscDown => SelectedAdviseSet != null && SelectedAdviseSetDisc != null && SelectedAdviseSetDiscIndex + 1 < CurrentAdviseSetDiscs.Count;

		public IAvailableDiscsViewModel AvailableDiscsViewModel { get; }

		private List<DiscModel> ActiveDiscs { get; set; }

		public ICommand CreateAdviseSetCommand { get; }

		public ICommand DeleteAdviseSetCommand { get; }

		public ICommand AddDiscsCommand { get; }

		public ICommand RemoveDiscCommand { get; }

		public ICommand MoveDiscUpCommand { get; }

		public ICommand MoveDiscDownCommand { get; }

		public AdviseSetsEditorViewModel(IAvailableDiscsViewModel availableDiscsViewModel, IAdviseSetService adviseSetService, IDiscsService discService, IWindowService windowService)
		{
			this.AvailableDiscsViewModel = availableDiscsViewModel ?? throw new ArgumentNullException(nameof(availableDiscsViewModel));
			this.adviseSetService = adviseSetService ?? throw new ArgumentNullException(nameof(adviseSetService));
			this.discService = discService ?? throw new ArgumentNullException(nameof(discService));
			this.windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));

			this.CreateAdviseSetCommand = new AsyncRelayCommand(() => CreateAdviseSet(CancellationToken.None));
			this.DeleteAdviseSetCommand = new AsyncRelayCommand(() => DeleteAdviseSet(CancellationToken.None));
			this.AddDiscsCommand = new AsyncRelayCommand(() => AddDiscs(CancellationToken.None));
			this.RemoveDiscCommand = new AsyncRelayCommand(() => RemoveDisc(CancellationToken.None));
			this.MoveDiscUpCommand = new AsyncRelayCommand(() => MoveDiscUp(CancellationToken.None));
			this.MoveDiscDownCommand = new AsyncRelayCommand(() => MoveDiscDown(CancellationToken.None));

			AvailableDiscsViewModel.PropertyChanged += AvailableDiscsViewModelOnPropertyChanged;
		}

		public async Task Load(CancellationToken cancellationToken)
		{
			await LoadAdviseSets(cancellationToken);

			var discs = await discService.GetAllDiscs(cancellationToken);
			ActiveDiscs = discs.Where(x => !x.IsDeleted).ToList();

			await AvailableDiscsViewModel.LoadDiscs(ActiveDiscs, cancellationToken);
		}

		public async Task RenameAdviseSet(AdviseSetModel adviseSet, CancellationToken cancellationToken)
		{
			await adviseSetService.UpdateAdviseSet(adviseSet, cancellationToken);

			await ReloadAdviseSets(reloadAvailableDiscs: false, cancellationToken);
		}

		private async Task LoadAdviseSets(CancellationToken cancellationToken)
		{
			var adviseSets = await adviseSetService.GetAllAdviseSets(cancellationToken);

			AdviseSets.Clear();
			AdviseSets.AddRange(adviseSets);
		}

		private Task ReloadAdviseSets(bool reloadAvailableDiscs, CancellationToken cancellationToken)
		{
			return ReloadAdviseSets(SelectedAdviseSet?.Id, reloadAvailableDiscs, cancellationToken);
		}

		private async Task ReloadAdviseSets(ItemId selectedItemId, bool reloadAvailableDiscs, CancellationToken cancellationToken)
		{
			await LoadAdviseSets(cancellationToken);

			if (reloadAvailableDiscs)
			{
				await AvailableDiscsViewModel.LoadDiscs(ActiveDiscs, cancellationToken);
			}

			// Selected advise set should be set after reloading available discs, because setter invokes LoadAvailableDiscsForAdviseSet().
			SelectedAdviseSet = selectedItemId != null ? AdviseSets.FirstOrDefault(x => x.Id == selectedItemId) : null;
			RaisePropertyChangedForAdviseSetButtons();
		}

		private async Task CreateAdviseSet(CancellationToken cancellationToken)
		{
			var discs = AvailableDiscsViewModel.SelectedDiscs.ToList();
			var newAdviseSet = new AdviseSetModel
			{
				Name = GetDefaultNameForNewAdviseSet(discs),
			};

			await adviseSetService.CreateAdviseSet(newAdviseSet, cancellationToken);

			if (discs.Any())
			{
				await adviseSetService.AddDiscs(newAdviseSet, discs, cancellationToken);
			}

			await ReloadAdviseSets(newAdviseSet.Id, reloadAvailableDiscs: true, cancellationToken);

			if (SelectedAdviseSet != null)
			{
				Messenger.Default.Send(new AdviseSetCreatedEventArgs(SelectedAdviseSet));
			}
		}

		private string GetDefaultNameForNewAdviseSet(IReadOnlyCollection<DiscModel> adviseSetDiscs)
		{
			var parentFolder = adviseSetDiscs
				.Select(x => x.Folder)
				.UniqueOrDefault(new ShallowFolderEqualityComparer());

			var folderPrefix = parentFolder != null ? $"{parentFolder.Name} / " : String.Empty;

			var albumTitle = adviseSetDiscs.Select(disc => disc.AlbumTitle).UniqueOrDefault(StringComparer.Ordinal);
			if (albumTitle != null)
			{
				return $"{folderPrefix}{albumTitle}";
			}

			var existingNames = AdviseSets.Select(x => x.Name).ToHashSet();

			// "New Advise Set", "New Advise Set (2)", "New Advise Set (3)", ...
			for (var i = 1; ; ++i)
			{
				var newAdviseSetName = $"{folderPrefix}New Advise Set";
				if (i > 1)
				{
					newAdviseSetName += $" ({i:N0})";
				}

				if (!existingNames.Contains(newAdviseSetName))
				{
					return newAdviseSetName;
				}
			}
		}

		private async Task DeleteAdviseSet(CancellationToken cancellationToken)
		{
			if (!CanDeleteAdviseSet)
			{
				return;
			}

			var result = windowService.ShowMessageBox($"Do you really want to delete advise set '{SelectedAdviseSet.Name}'?", "Delete advise set?", ShowMessageBoxButton.YesNo, ShowMessageBoxIcon.Warning);
			if (result != ShowMessageBoxResult.Yes)
			{
				return;
			}

			await adviseSetService.DeleteAdviseSet(SelectedAdviseSet, cancellationToken);

			// Clearing advise set for in-memory disc models.
			foreach (var disc in CurrentAdviseSetDiscs)
			{
				disc.AdviseSetInfo = null;
			}

			await ReloadAdviseSets(reloadAvailableDiscs: true, cancellationToken);
		}

		private async Task AddDiscs(CancellationToken cancellationToken)
		{
			if (!CanAddDiscs)
			{
				return;
			}

			await adviseSetService.AddDiscs(SelectedAdviseSet, AvailableDiscsViewModel.SelectedDiscs, cancellationToken);

			await ReloadAdviseSets(reloadAvailableDiscs: true, cancellationToken);
		}

		private async Task RemoveDisc(CancellationToken cancellationToken)
		{
			if (!CanRemoveDisc)
			{
				return;
			}

			await adviseSetService.RemoveDiscs(SelectedAdviseSet, new[] { SelectedAdviseSetDisc }, cancellationToken);

			await ReloadAdviseSets(reloadAvailableDiscs: true, cancellationToken);
		}

		private async Task MoveDiscUp(CancellationToken cancellationToken)
		{
			if (!CanMoveDiscUp)
			{
				return;
			}

			await MoveSelectedDisc(-1, cancellationToken);
		}

		private async Task MoveDiscDown(CancellationToken cancellationToken)
		{
			if (!CanMoveDiscDown)
			{
				return;
			}

			await MoveSelectedDisc(+1, cancellationToken);
		}

		private async Task MoveSelectedDisc(int indexChange, CancellationToken cancellationToken)
		{
			var selectedDiscId = SelectedAdviseSetDisc.Id;

			var oldSelectedDiscIndex = SelectedAdviseSetDiscIndex;
			var newSelectedDiscIndex = oldSelectedDiscIndex + indexChange;

			var discs = new List<DiscModel>(CurrentAdviseSetDiscs)
			{
				[oldSelectedDiscIndex] = CurrentAdviseSetDiscs[newSelectedDiscIndex],
				[newSelectedDiscIndex] = CurrentAdviseSetDiscs[oldSelectedDiscIndex],
			};

			await adviseSetService.ReorderDiscs(SelectedAdviseSet, discs, cancellationToken);

			await ReloadAdviseSets(reloadAvailableDiscs: false, cancellationToken);

			SelectedAdviseSetDisc = CurrentAdviseSetDiscs.FirstOrDefault(x => x.Id == selectedDiscId);
		}

		private void RaisePropertyChangedForAdviseSetButtons()
		{
			RaisePropertyChanged(nameof(CanDeleteAdviseSet));
			RaisePropertyChanged(nameof(CanAddDiscs));
			RaisePropertyChanged(nameof(CanRemoveDisc));
			RaisePropertyChanged(nameof(CanMoveDiscUp));
			RaisePropertyChanged(nameof(CanMoveDiscDown));
		}

		private void AvailableDiscsViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(AvailableDiscsViewModel.SelectedItems))
			{
				RaisePropertyChanged(nameof(CanCreateAdviseSet));
				RaisePropertyChanged(nameof(CanAddDiscs));
			}
		}
	}
}
