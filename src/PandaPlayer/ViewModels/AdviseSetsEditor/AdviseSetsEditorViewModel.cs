using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

		private readonly IFoldersService folderService;

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

				RaisePropertyChangedForAdviseSetButtons();
			}
		}

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

		public bool CanAddDisc => SelectedAdviseSet != null && SelectedAvailableDiscsForAdding.Any();

		public bool CanRemoveDisc => SelectedAdviseSet != null && SelectedAdviseSetDisc != null;

		public bool CanMoveDiscUp => SelectedAdviseSet != null && SelectedAdviseSetDisc != null && SelectedAdviseSetDiscIndex != 0;

		public bool CanMoveDiscDown => SelectedAdviseSet != null && SelectedAdviseSetDisc != null && SelectedAdviseSetDiscIndex + 1 < CurrentAdviseSetDiscs.Count;

		private List<DiscModel> ActiveDiscs { get; set; }

		private List<AvailableDiscViewModel> availableDiscs;

		public IReadOnlyCollection<AvailableDiscViewModel> AvailableDiscs => availableDiscs;

		private IList selectedAvailableDiscItems;

		public IList SelectedAvailableDiscItems
		{
			get => selectedAvailableDiscItems;
			set
			{
				Set(ref selectedAvailableDiscItems, value);

				RaisePropertyChanged(nameof(CanAddDisc));
			}
		}

		private IEnumerable<AvailableDiscViewModel> SelectedAvailableDiscs => SelectedAvailableDiscItems?.Cast<AvailableDiscViewModel>() ?? Enumerable.Empty<AvailableDiscViewModel>();

		private IEnumerable<AvailableDiscViewModel> SelectedAvailableDiscsForAdding => SelectedAvailableDiscs
			.Where(selectedAvailableDisc => !CurrentAdviseSetDiscs.Contains(selectedAvailableDisc.Disc, new DiscEqualityComparer()));

		public ICommand CreateAdviseSetCommand { get; }

		public ICommand DeleteAdviseSetCommand { get; }

		public ICommand AddDiscsCommand { get; }

		public ICommand RemoveDiscCommand { get; }

		public ICommand MoveDiscUpCommand { get; }

		public ICommand MoveDiscDownCommand { get; }

		public AdviseSetsEditorViewModel(IAdviseSetService adviseSetService, IDiscsService discService, IFoldersService folderService, IWindowService windowService)
		{
			this.adviseSetService = adviseSetService ?? throw new ArgumentNullException(nameof(adviseSetService));
			this.discService = discService ?? throw new ArgumentNullException(nameof(discService));
			this.folderService = folderService ?? throw new ArgumentNullException(nameof(folderService));
			this.windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));

			this.CreateAdviseSetCommand = new AsyncRelayCommand(() => CreateAdviseSet(CancellationToken.None));
			this.DeleteAdviseSetCommand = new AsyncRelayCommand(() => DeleteAdviseSet(CancellationToken.None));
			this.AddDiscsCommand = new AsyncRelayCommand(() => AddDiscs(CancellationToken.None));
			this.RemoveDiscCommand = new AsyncRelayCommand(() => RemoveDisc(CancellationToken.None));
			this.MoveDiscUpCommand = new AsyncRelayCommand(() => MoveDiscUp(CancellationToken.None));
			this.MoveDiscDownCommand = new AsyncRelayCommand(() => MoveDiscDown(CancellationToken.None));
		}

		public async Task Load(CancellationToken cancellationToken)
		{
			await LoadAdviseSets(cancellationToken);

			var discs = await discService.GetAllDiscs(cancellationToken);
			var folders = await folderService.GetAllFolders(cancellationToken);

			ActiveDiscs = discs.Where(x => !x.IsDeleted).ToList();

			availableDiscs = ActiveDiscs
				.Select(x => new AvailableDiscViewModel(x, GetAvailableDiscTitle(x, folders)))
				.OrderBy(x => x.Title, StringComparer.InvariantCultureIgnoreCase)
				.ToList();
		}

		public async Task RenameAdviseSet(AdviseSetModel adviseSet, CancellationToken cancellationToken)
		{
			await adviseSetService.UpdateAdviseSet(adviseSet, cancellationToken);

			await ReloadAdviseSets(cancellationToken);
		}

		private async Task LoadAdviseSets(CancellationToken cancellationToken)
		{
			var adviseSets = await adviseSetService.GetAllAdviseSets(cancellationToken);

			AdviseSets.Clear();
			AdviseSets.AddRange(adviseSets);
		}

		private Task ReloadAdviseSets(CancellationToken cancellationToken)
		{
			return ReloadAdviseSets(SelectedAdviseSet?.Id, cancellationToken);
		}

		private async Task ReloadAdviseSets(ItemId selectedItemId, CancellationToken cancellationToken)
		{
			await LoadAdviseSets(cancellationToken);

			SelectedAdviseSet = selectedItemId != null ? AdviseSets.FirstOrDefault(x => x.Id == selectedItemId) : null;
			RaisePropertyChangedForAdviseSetButtons();
		}

		private static string GetAvailableDiscTitle(DiscModel disc, IReadOnlyCollection<ShallowFolderModel> folders)
		{
			var foldersMap = folders.ToDictionary(x => x.Id, x => x);

			var folderNames = new List<string>();
			for (var folder = disc.Folder; !folder.IsRoot; folder = foldersMap[folder.ParentFolderId])
			{
				folderNames.Add(folder.Name);
			}

			folderNames.Reverse();

			return $"/{String.Join("/", folderNames)}/{disc.TreeTitle}";
		}

		private async Task CreateAdviseSet(CancellationToken cancellationToken)
		{
			var discs = SelectedAvailableDiscsForAdding.Select(x => x.Disc).ToList();
			var adviseSetName = discs.Select(disc => disc.AlbumTitle).UniqueOrDefault(StringComparer.Ordinal) ?? "New Advise Set";
			var newAdviseSet = new AdviseSetModel
			{
				Name = adviseSetName,
			};

			await adviseSetService.CreateAdviseSet(newAdviseSet, cancellationToken);

			if (discs.Any())
			{
				await adviseSetService.AddDiscs(newAdviseSet, discs, cancellationToken);
			}

			await ReloadAdviseSets(newAdviseSet.Id, cancellationToken);

			if (SelectedAdviseSet != null)
			{
				Messenger.Default.Send(new AdviseSetCreatedEventArgs(SelectedAdviseSet));
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

			await ReloadAdviseSets(cancellationToken);
		}

		private async Task AddDiscs(CancellationToken cancellationToken)
		{
			if (!CanAddDisc)
			{
				return;
			}

			await adviseSetService.AddDiscs(SelectedAdviseSet, SelectedAvailableDiscsForAdding.Select(x => x.Disc), cancellationToken);

			await ReloadAdviseSets(cancellationToken);
		}

		private async Task RemoveDisc(CancellationToken cancellationToken)
		{
			if (!CanRemoveDisc)
			{
				return;
			}

			await adviseSetService.RemoveDiscs(SelectedAdviseSet, new[] { SelectedAdviseSetDisc }, cancellationToken);

			await ReloadAdviseSets(cancellationToken);
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

			await ReloadAdviseSets(cancellationToken);

			SelectedAdviseSetDisc = CurrentAdviseSetDiscs.FirstOrDefault(x => x.Id == selectedDiscId);
		}

		private void RaisePropertyChangedForAdviseSetButtons()
		{
			RaisePropertyChanged(nameof(CanDeleteAdviseSet));
			RaisePropertyChanged(nameof(CanAddDisc));
			RaisePropertyChanged(nameof(CanRemoveDisc));
			RaisePropertyChanged(nameof(CanMoveDiscUp));
			RaisePropertyChanged(nameof(CanMoveDiscDown));
		}
	}
}
