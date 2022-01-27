using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Comparers;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Services.Interfaces.Dal;
using PandaPlayer.Services.Internal;

namespace PandaPlayer.Services
{
	internal class AdviseSetService : IAdviseSetService
	{
		private readonly IAdviseSetRepository adviseSetRepository;

		private readonly IDiscsRepository discsRepository;

		private static IDiscLibrary DiscLibrary => DiscLibraryHolder.DiscLibrary;

		public AdviseSetService(IAdviseSetRepository adviseSetRepository, IDiscsRepository discsRepository)
		{
			this.adviseSetRepository = adviseSetRepository ?? throw new ArgumentNullException(nameof(adviseSetRepository));
			this.discsRepository = discsRepository ?? throw new ArgumentNullException(nameof(discsRepository));
		}

		public async Task CreateAdviseSet(AdviseSetModel adviseSet, CancellationToken cancellationToken)
		{
			await adviseSetRepository.CreateAdviseSet(adviseSet, cancellationToken);

			DiscLibrary.AddAdviseSet(adviseSet);
		}

		public Task<IReadOnlyCollection<AdviseSetModel>> GetAllAdviseSets(CancellationToken cancellationToken)
		{
			return Task.FromResult(DiscLibrary.AdviseSets);
		}

		public async Task AddDiscs(AdviseSetModel adviseSet, IEnumerable<DiscModel> addedDiscs, CancellationToken cancellationToken)
		{
			var lastAdviseSetInfo = GetAdviseSetDiscs(adviseSet).LastOrDefault()?.AdviseSetInfo;
			var nextOrder = (lastAdviseSetInfo?.Order ?? 0) + 1;
			foreach (var addedDisc in addedDiscs)
			{
				addedDisc.AdviseSetInfo = new AdviseSetInfo(adviseSet, nextOrder++);
				await discsRepository.UpdateDisc(addedDisc, cancellationToken);
			}
		}

		public async Task RemoveDiscs(AdviseSetModel adviseSet, IEnumerable<DiscModel> removedDiscs, CancellationToken cancellationToken)
		{
			var removedDiscsMap = removedDiscs.ToDictionary(x => x.Id, x => x);

			var nextOrder = 1;
			foreach (var disc in GetAdviseSetDiscs(adviseSet))
			{
				if (removedDiscsMap.TryGetValue(disc.Id, out var removedDisc))
				{
					disc.AdviseSetInfo = null;
					await discsRepository.UpdateDisc(disc, cancellationToken);

					removedDisc.AdviseSetInfo = null;
				}
				else
				{
					if (disc.AdviseSetInfo.Order != nextOrder)
					{
						disc.AdviseSetInfo = disc.AdviseSetInfo.WithOrder(nextOrder);
						await discsRepository.UpdateDisc(disc, cancellationToken);
					}

					++nextOrder;
				}
			}
		}

		public async Task ReorderDiscs(AdviseSetModel adviseSet, IEnumerable<DiscModel> newDiscsOrder, CancellationToken cancellationToken)
		{
			var currentDiscs = GetAdviseSetDiscs(adviseSet).ToList();
			var newDiscsOrderList = newDiscsOrder.ToList();

			var oldOrders = currentDiscs.ToDictionary(x => x.Id, x => x.AdviseSetInfo.Order);
			var newOrders = newDiscsOrderList
				.Select((x, i) => (Disc: x, Order: i + 1))
				.ToDictionary(x => x.Disc.Id, x => x.Order);

			var discsChanged = currentDiscs.Except(newDiscsOrderList, new DiscEqualityComparer()).Any() ||
			                   newDiscsOrderList.Except(currentDiscs, new DiscEqualityComparer()).Any();
			if (discsChanged)
			{
				throw new InvalidOperationException("Can not reorder advise set discs because advise set was modified");
			}

			// For satisfying unique order constraint, for discs with changed order we have to clear existing order at first.
			foreach (var disc in newDiscsOrderList)
			{
				if (newOrders[disc.Id] != oldOrders[disc.Id])
				{
					disc.AdviseSetInfo = null;
					await discsRepository.UpdateDisc(disc, cancellationToken);
				}
			}

			// Finally saving new order.
			foreach (var disc in newDiscsOrderList)
			{
				if (disc.AdviseSetInfo == null)
				{
					disc.AdviseSetInfo = new AdviseSetInfo(adviseSet, newOrders[disc.Id]);
					await discsRepository.UpdateDisc(disc, cancellationToken);
				}
			}
		}

		public Task UpdateAdviseSet(AdviseSetModel adviseSet, CancellationToken cancellationToken)
		{
			return adviseSetRepository.UpdateAdviseSet(adviseSet, cancellationToken);
		}

		public async Task DeleteAdviseSet(AdviseSetModel adviseSet, CancellationToken cancellationToken)
		{
			var currentDiscs = GetAdviseSetDiscs(adviseSet);

			foreach (var disc in currentDiscs)
			{
				disc.AdviseSetInfo = null;
				await discsRepository.UpdateDisc(disc, cancellationToken);
			}

			await adviseSetRepository.DeleteAdviseSet(adviseSet, cancellationToken);

			DiscLibrary.DeleteAdviseSet(adviseSet);
		}

		private static IReadOnlyCollection<DiscModel> GetAdviseSetDiscs(AdviseSetModel adviseSet)
		{
			return DiscLibrary.Discs
				.Where(x => x.AdviseSetInfo?.AdviseSet.Id == adviseSet.Id)
				.OrderBy(x => x.AdviseSetInfo.Order)
				.ToList();
		}
	}
}
