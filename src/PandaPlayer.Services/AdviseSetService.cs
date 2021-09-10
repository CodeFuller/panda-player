﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Comparers;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Services.Interfaces.Dal;

namespace PandaPlayer.Services
{
	internal class AdviseSetService : IAdviseSetService
	{
		private readonly IAdviseSetRepository adviseSetRepository;

		private readonly IDiscsRepository discsRepository;

		public AdviseSetService(IAdviseSetRepository adviseSetRepository, IDiscsRepository discsRepository)
		{
			this.adviseSetRepository = adviseSetRepository ?? throw new ArgumentNullException(nameof(adviseSetRepository));
			this.discsRepository = discsRepository ?? throw new ArgumentNullException(nameof(discsRepository));
		}

		public Task CreateAdviseSet(AdviseSetModel adviseSet, CancellationToken cancellationToken)
		{
			return adviseSetRepository.CreateAdviseSet(adviseSet, cancellationToken);
		}

		public async Task<IReadOnlyCollection<AdviseSetModel>> GetAllAdviseSets(CancellationToken cancellationToken)
		{
			return (await adviseSetRepository.GetAllAdviseSets(cancellationToken))
				.OrderBy(ag => ag.Name)
				.ToList();
		}

		public async Task AddDiscs(AdviseSetModel adviseSet, IEnumerable<DiscModel> addedDiscs, CancellationToken cancellationToken)
		{
			var currentDiscs = await discsRepository.GetAdviseSetDiscs(adviseSet.Id, cancellationToken);

			var lastAdviseSetInfo = currentDiscs.LastOrDefault()?.AdviseSetInfo;
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

			var currentDiscs = await discsRepository.GetAdviseSetDiscs(adviseSet.Id, cancellationToken);

			var nextOrder = 1;
			foreach (var disc in currentDiscs)
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
			var newDiscsOrderList = newDiscsOrder.ToList();

			var currentDiscs = await discsRepository.GetAdviseSetDiscs(adviseSet.Id, cancellationToken);

			var discsChanged = currentDiscs.Except(newDiscsOrderList, new DiscEqualityComparer()).Any() ||
			                   newDiscsOrderList.Except(currentDiscs, new DiscEqualityComparer()).Any();
			if (discsChanged)
			{
				throw new InvalidOperationException("Can not reorder advise set discs because advise set was modified");
			}

			var newOrders = new Dictionary<ItemId, int>();

			// Updating input models.
			foreach (var (disc, order) in newDiscsOrderList.Select((disc, i) => (Disc: disc, Order: i + 1)))
			{
				disc.AdviseSetInfo = new AdviseSetInfo(adviseSet, order);
				newOrders.Add(disc.Id, order);
			}

			// For satisfying unique order constraint, for discs with changed order we have to clear existing order at first.
			foreach (var disc in currentDiscs)
			{
				var newOrder = newOrders[disc.Id];
				if (disc.AdviseSetInfo.Order != newOrder)
				{
					disc.AdviseSetInfo = null;
					await discsRepository.UpdateDisc(disc, cancellationToken);
				}
			}

			// Finally saving new order.
			foreach (var disc in currentDiscs)
			{
				var newOrder = newOrders[disc.Id];
				if (disc.AdviseSetInfo == null)
				{
					disc.AdviseSetInfo = new AdviseSetInfo(adviseSet, newOrder);
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
			var currentDiscs = await discsRepository.GetAdviseSetDiscs(adviseSet.Id, cancellationToken);

			foreach (var disc in currentDiscs)
			{
				disc.AdviseSetInfo = null;
				await discsRepository.UpdateDisc(disc, cancellationToken);
			}

			await adviseSetRepository.DeleteAdviseSet(adviseSet, cancellationToken);
		}
	}
}
