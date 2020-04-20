using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Interfaces;
using MusicLibrary.Core.Objects;
using MusicLibrary.Dal.Abstractions.Dto;
using MusicLibrary.Dal.Abstractions.Interfaces;
using MusicLibrary.Dal.LocalDb.Extensions;

namespace MusicLibrary.Dal.LocalDb
{
	public class DiscsRepository : IDiscsRepository
	{
		private readonly IMusicLibrary musicLibrary;

		private readonly DiscLibrary discLibrary;

		public DiscsRepository(IMusicLibrary musicLibrary, DiscLibrary discLibrary)
		{
			this.musicLibrary = musicLibrary ?? throw new ArgumentNullException(nameof(musicLibrary));
			this.discLibrary = discLibrary ?? throw new ArgumentNullException(nameof(discLibrary));
		}

		public Task DeleteDisc(ItemId discId, CancellationToken cancellationToken)
		{
			var disc = FindDisc(discId);
			return musicLibrary.DeleteDisc(disc);
		}

		private Disc FindDisc(ItemId discId)
		{
			return discLibrary.Discs.Single(d => d.Uri.ToItemId() == discId);
		}
	}
}
