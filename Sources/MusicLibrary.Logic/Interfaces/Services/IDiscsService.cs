﻿using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Logic.Interfaces.Services
{
	public interface IDiscsService
	{
		Task<DiscModel> GetDisc(ItemId discId, CancellationToken cancellationToken);

		Task UpdateDisc(DiscModel disc, CancellationToken cancellationToken);

		Task DeleteDisc(ItemId discId, CancellationToken cancellationToken);
	}
}
