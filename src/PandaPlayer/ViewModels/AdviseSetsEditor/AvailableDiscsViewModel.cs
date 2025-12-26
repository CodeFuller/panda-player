using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using PandaPlayer.Core.Comparers;
using PandaPlayer.Core.Models;
using PandaPlayer.Shared.Extensions;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels.AdviseSetsEditor
{
	public class AvailableDiscsViewModel : ObservableObject, IAvailableDiscsViewModel
	{
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
				OnPropertyChanged(nameof(SelectedItems));
			}
		}

		public IEnumerable<DiscModel> SelectedDiscs => SelectedItems?.Cast<AvailableDiscViewModel>().Select(x => x.Disc) ?? Enumerable.Empty<DiscModel>();

		public Task LoadDiscs(IEnumerable<DiscModel> activeLibraryDiscs, CancellationToken cancellationToken)
		{
			AllAvailableDiscs = activeLibraryDiscs
				.Where(x => x.AdviseSetInfo == null)
				.Select(x => new AvailableDiscViewModel(x, GetAvailableDiscTitle(x)))
				.OrderBy(x => x.Title, StringComparer.InvariantCultureIgnoreCase)
				.ToList();

			AvailableDiscs.Clear();
			AvailableDiscs.AddRange(AllAvailableDiscs);

			return Task.CompletedTask;
		}

		public void LoadAvailableDiscsForAdviseSet(IReadOnlyCollection<DiscModel> adviseSetDiscs)
		{
			AvailableDiscs.Clear();
			AvailableDiscs.AddRange(AllAvailableDiscs.Where(x => DiscIsAvailableForAdviseSet(x.Disc, adviseSetDiscs)));
		}

		public bool SelectedDiscsCanBeAddedToAdviseSet(IReadOnlyCollection<DiscModel> adviseSetDiscs)
		{
			var selectedDiscs = SelectedDiscs.ToList();

			if (selectedDiscs.Count == 0)
			{
				return false;
			}

			return DiscsAreValidForAdviseSet(selectedDiscs.Concat(adviseSetDiscs).ToList());
		}

		private static bool DiscIsAvailableForAdviseSet(DiscModel disc, IReadOnlyCollection<DiscModel> adviseSetDiscs)
		{
			// Disc is not available for advise set if it already has assigned advise set.
			if (disc.AdviseSetInfo != null)
			{
				return false;
			}

			// Disc is available for advise set if this advise set is empty.
			if (adviseSetDiscs.Count == 0)
			{
				return true;
			}

			return DiscsAreValidForAdviseSet(new[] { disc }.Concat(adviseSetDiscs).ToList());
		}

		private static bool DiscsAreValidForAdviseSet(IReadOnlyCollection<DiscModel> discs)
		{
			var parentFolders = discs
				.Select(x => x.Folder)
				.Distinct(new FolderEqualityComparer());

			if (parentFolders.Count() > 1)
			{
				return false;
			}

			var adviseGroups = discs
				.Select(x => x.AdviseGroup)
				.Distinct(new AdviseGroupEqualityComparer());

			return adviseGroups.Count() <= 1;
		}

		private static string GetAvailableDiscTitle(DiscModel disc)
		{
			return $"/{String.Join("/", disc.Folder.PathNames)}/{disc.TreeTitle}";
		}
	}
}
