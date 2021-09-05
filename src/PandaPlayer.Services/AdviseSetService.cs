using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;

namespace PandaPlayer.Services
{
	// TODO: Implement DAL for advise sets.
	internal class AdviseSetService : IAdviseSetService
	{
		private readonly List<AdviseSetModel> adviseSets = new()
		{
			new AdviseSetModel
			{
				Id = new ItemId("1"),
				Name = "Advise Set 1",
			},

			new AdviseSetModel
			{
				Id = new ItemId("2"),
				Name = "Advise Set 2",
			},

			new AdviseSetModel
			{
				Id = new ItemId("3"),
				Name = "Advise Set 3",
			},
		};

		public Task CreateAdviseSet(AdviseSetModel adviseSet, CancellationToken cancellationToken)
		{
			adviseSet.Id = new ItemId((adviseSets.Select(x => Int32.Parse(x.Id.Value, NumberStyles.None, CultureInfo.InvariantCulture)).Max() + 1).ToString("n0", CultureInfo.InvariantCulture));
			adviseSets.Add(adviseSet);
			adviseSets.Sort((x, y) => String.CompareOrdinal(x.Name, y.Name));

			return Task.CompletedTask;
		}

		public Task<IReadOnlyCollection<AdviseSetModel>> GetAllAdviseSets(CancellationToken cancellationToken)
		{
			return Task.FromResult<IReadOnlyCollection<AdviseSetModel>>(adviseSets);
		}

		public Task SetAdviseSetDiscs(AdviseSetModel adviseSet, IReadOnlyCollection<DiscModel> discs, CancellationToken cancellationToken)
		{
			foreach (var (disc, order) in discs.Select((disc, i) => (disc, i + 1)))
			{
				disc.AdviseSetOrder = order;
			}

			adviseSet.Discs = discs.ToList();

			return Task.CompletedTask;
		}

		public Task UpdateAdviseSet(AdviseSetModel adviseSet, CancellationToken cancellationToken)
		{
			adviseSets.Sort((x, y) => String.CompareOrdinal(x.Name, y.Name));

			return Task.CompletedTask;
		}

		public Task DeleteAdviseSet(AdviseSetModel adviseSet, CancellationToken cancellationToken)
		{
			adviseSets.Remove(adviseSet);

			return Task.CompletedTask;
		}
	}
}
