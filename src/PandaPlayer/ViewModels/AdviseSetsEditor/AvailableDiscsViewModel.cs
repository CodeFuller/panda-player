using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Shared.Extensions;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels.AdviseSetsEditor
{
	public class AvailableDiscsViewModel : ViewModelBase, IAvailableDiscsViewModel
	{
		private readonly IFoldersService folderService;

		public ObservableCollection<AvailableDiscViewModel> AvailableDiscs { get; } = new();

		private IList selectedItems;

#pragma warning disable CA2227 // Collection properties should be read only
		public IList SelectedItems
#pragma warning restore CA2227 // Collection properties should be read only
		{
			get => selectedItems;
			set
			{
				Set(ref selectedItems, value);
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

			var availableDiscs = activeLibraryDiscs
				.Where(x => x.AdviseSetInfo == null)
				.Select(x => new AvailableDiscViewModel(x, GetAvailableDiscTitle(x, folders)))
				.OrderBy(x => x.Title, StringComparer.InvariantCultureIgnoreCase);

			AvailableDiscs.Clear();
			AvailableDiscs.AddRange(availableDiscs);
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
