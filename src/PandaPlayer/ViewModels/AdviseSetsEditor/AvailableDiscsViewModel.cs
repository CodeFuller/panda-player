using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PandaPlayer.Core.Comparers;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Shared.Extensions;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels.AdviseSetsEditor
{
	public class AvailableDiscsViewModel : ViewModelBase, IAvailableDiscsViewModel
	{
		private readonly IFoldersService folderService;

		private IReadOnlyCollection<AvailableDiscViewModel> AllAvailableDiscs { get; set; }

		public ObservableCollection<AvailableDiscViewModel> AvailableDiscs { get; } = new();

		private IList selectedItems;

#pragma warning disable CA2227 // Collection properties should be read only
		public IList SelectedItems
#pragma warning restore CA2227 // Collection properties should be read only
		{
			get => selectedItems;
			set
			{
				selectedItems = value;

				// We do not use Set() helper and raise PropertyChanged manually.
				// On subsequent calls, selectedItems and value will reference the same collection.
				// ViewModelBase.Set() method does not raise PropertyChanged in such case.
				RaisePropertyChanged(nameof(SelectedItems));
			}
		}

		public IEnumerable<DiscModel> SelectedDiscs => SelectedItems?.Cast<AvailableDiscViewModel>().Select(x => x.Disc) ?? Enumerable.Empty<DiscModel>();

		public AvailableDiscsViewModel(IFoldersService folderService)
		{
			this.folderService = folderService ?? throw new ArgumentNullException(nameof(folderService));
		}

		public async Task LoadDiscs(IEnumerable<DiscModel> activeLibraryDiscs, CancellationToken cancellationToken)
		{
			var folders = await folderService.GetAllFolders(cancellationToken);

			AllAvailableDiscs = activeLibraryDiscs
				.Where(x => x.AdviseSetInfo == null)
				.Select(x => new AvailableDiscViewModel(x, GetAvailableDiscTitle(x, folders)))
				.OrderBy(x => x.Title, StringComparer.InvariantCultureIgnoreCase)
				.ToList();

			AvailableDiscs.Clear();
			AvailableDiscs.AddRange(AllAvailableDiscs);
		}

		public void LoadAvailableDiscsForAdviseSet(IReadOnlyCollection<DiscModel> adviseSetDiscs)
		{
			AvailableDiscs.Clear();
			AvailableDiscs.AddRange(AllAvailableDiscs.Where(x => DiscIsAvailableForAdviseSet(x.Disc, adviseSetDiscs)));
		}

		private static bool DiscIsAvailableForAdviseSet(DiscModel disc, IReadOnlyCollection<DiscModel> adviseSetDiscs)
		{
			// Disc is not available for advise set if it already has assigned advise set.
			if (disc.AdviseSetInfo != null)
			{
				return false;
			}

			// Disc is available for advise set if this advise set is empty.
			if (!adviseSetDiscs.Any())
			{
				return true;
			}

			if (disc.AdviseGroup != null)
			{
				var adviseGroupForAdviseSetDiscs = adviseSetDiscs
					.Select(x => x.AdviseGroup)
					.Distinct(new AdviseGroupEqualityComparer())
					.Single();

				// Disc is available for advise set if disc and advise set have same advise group.
				return disc.AdviseGroup.Id == adviseGroupForAdviseSetDiscs?.Id;
			}

			var parentFolderForAdviseSetDiscs = adviseSetDiscs
				.Select(x => x.Folder)
				.Distinct(new ShallowFolderEqualityComparer())
				.Single();

			// Disc is available for advise set if it does not assigned advise group and all discs belong to the same folder.
			// Note: this logic is not 100% accurate. disc or adviseSetDiscs can have assigned advise group on higher level (for some directory).
			// We do not cover this case for simplicity. It is assumed that advise set will contain discs from the same directory.
			return disc.Folder.Id == parentFolderForAdviseSetDiscs.Id;
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
	}
}
