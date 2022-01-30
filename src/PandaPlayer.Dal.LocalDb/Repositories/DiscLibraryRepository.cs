using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PandaPlayer.Core.Models;
using PandaPlayer.Dal.LocalDb.Entities;
using PandaPlayer.Dal.LocalDb.Extensions;
using PandaPlayer.Dal.LocalDb.Interfaces;
using PandaPlayer.Dal.LocalDb.Internal;
using PandaPlayer.Services.Interfaces.Dal;
using PandaPlayer.Services.Internal;

namespace PandaPlayer.Dal.LocalDb.Repositories
{
	internal class DiscLibraryRepository : IDiscLibraryRepository
	{
		private readonly IDbContextFactory<MusicDbContext> contextFactory;

		private readonly IContentUriProvider contentUriProvider;

		public DiscLibraryRepository(IDbContextFactory<MusicDbContext> contextFactory, IContentUriProvider contentUriProvider)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
			this.contentUriProvider = contentUriProvider ?? throw new ArgumentNullException(nameof(contentUriProvider));
		}

		public Task<DiscLibrary> ReadDiscLibrary(CancellationToken cancellationToken)
		{
			return ReadDiscLibrary(readPlaybacks: false, cancellationToken);
		}

		public Task<DiscLibrary> ReadDiscLibraryWithPlaybacks(CancellationToken cancellationToken)
		{
			return ReadDiscLibrary(readPlaybacks: true, cancellationToken);
		}

		private async Task<DiscLibrary> ReadDiscLibrary(bool readPlaybacks, CancellationToken cancellationToken)
		{
			await using var context = contextFactory.CreateDbContext();

			var adviseGroupEntities = await context.AdviseGroups.ToListAsync(cancellationToken);
			var folderEntities = await context.Folders.ToListAsync(cancellationToken);
			var adviseSetEntities = await context.AdviseSets.ToListAsync(cancellationToken);
			var discEntities = await context.Discs.ToListAsync(cancellationToken);
			var discImageEntities = await context.DiscImages.ToListAsync(cancellationToken);
			var artistEntities = await context.Artists.ToListAsync(cancellationToken);
			var genreEntities = await context.Genres.ToListAsync(cancellationToken);

			IQueryable<SongEntity> songsQueryable = context.Songs;
			if (readPlaybacks)
			{
				songsQueryable = songsQueryable.Include(x => x.Playbacks);
			}

			var songEntities = await songsQueryable.ToListAsync(cancellationToken);

			var adviseGroupModels = adviseGroupEntities
				.Select(x => x.ToModel())
				.ToDictionary(x => x.Id, x => x);

			var folderModels = CreateFolderModels(folderEntities, adviseGroupModels);

			var adviseSetModels = adviseSetEntities
				.Select(x => x.ToModel())
				.ToDictionary(x => x.Id, x => x);

			var discModels = CreateDiscModels(discEntities, folderModels, adviseGroupModels, adviseSetModels)
				.ToDictionary(x => x.Id, x => x);

			LinkDiscImagesToDiscs(discImageEntities, discModels);

			var artistModels = artistEntities
				.Select(x => x.ToModel())
				.ToDictionary(x => x.Id, x => x);

			var genreModels = genreEntities
				.Select(x => x.ToModel())
				.ToDictionary(x => x.Id, x => x);

			var songModels = CreateSongModels(songEntities, discModels, artistModels, genreModels);

			return new DiscLibrary(folderModels.Values, discModels.Values, songModels, artistModels.Values, genreModels.Values, adviseGroupModels.Values, adviseSetModels.Values);
		}

		private static IReadOnlyDictionary<ItemId, FolderModel> CreateFolderModels(IEnumerable<FolderEntity> folderEntities, IReadOnlyDictionary<ItemId, AdviseGroupModel> adviseGroups)
		{
			var entitiesMap = folderEntities.ToDictionary(x => x.Id.ToItemId(), x => x);

			var folders = entitiesMap.Values
				.Select(x => x.ToModel())
				.ToDictionary(x => x.Id, x => x);

			foreach (var folderModel in folders.Values)
			{
				var folderEntity = entitiesMap[folderModel.Id];
				var parentFolderId = folderEntity.ParentFolderId?.ToItemId();
				if (parentFolderId != null)
				{
					var parentFolder = folders[parentFolderId];
					parentFolder.AddSubfolder(folderModel);
				}

				folderModel.AdviseGroup = folderEntity.AdviseGroupId != null ? adviseGroups[folderEntity.AdviseGroupId.Value.ToItemId()] : null;
			}

			return folders;
		}

		private static IEnumerable<DiscModel> CreateDiscModels(IEnumerable<DiscEntity> discEntities, IReadOnlyDictionary<ItemId, FolderModel> folders,
			IReadOnlyDictionary<ItemId, AdviseGroupModel> adviseGroups, IReadOnlyDictionary<ItemId, AdviseSetModel> adviseSets)
		{
			foreach (var discEntity in discEntities)
			{
				var discModel = discEntity.ToModel();

				var parentFolder = folders[discEntity.FolderId.ToItemId()];
				parentFolder.AddDisc(discModel);

				discModel.AdviseGroup = discEntity.AdviseGroupId != null ? adviseGroups[discEntity.AdviseGroupId.Value.ToItemId()] : null;

				if (discEntity.AdviseSetId != null)
				{
					if (discEntity.AdviseSetOrder == null)
					{
						throw new InvalidOperationException($"Advise set order is not set for disc with advise set - '{discModel.TreeTitle}'");
					}

					discModel.AdviseSetInfo = new AdviseSetInfo(adviseSets[discEntity.AdviseSet.Id.ToItemId()], discEntity.AdviseSetOrder.Value);
				}
				else if (discEntity.AdviseSetOrder != null)
				{
					throw new InvalidOperationException($"Advise set order is set for disc without advise set - '{discModel.TreeTitle}'");
				}

				yield return discModel;
			}
		}

		private void LinkDiscImagesToDiscs(IEnumerable<DiscImageEntity> discImageEntities, IReadOnlyDictionary<ItemId, DiscModel> discModels)
		{
			foreach (var discImageEntity in discImageEntities)
			{
				var discImageModel = discImageEntity.ToModel();

				var parentDisc = discModels[discImageEntity.DiscId.ToItemId()];
				parentDisc.AddImage(discImageModel);

				contentUriProvider.SetDiscImageUri(discImageModel);
			}
		}

		private IEnumerable<SongModel> CreateSongModels(IEnumerable<SongEntity> songEntities, IReadOnlyDictionary<ItemId, DiscModel> discs,
			IReadOnlyDictionary<ItemId, ArtistModel> artists, IReadOnlyDictionary<ItemId, GenreModel> genres)
		{
			foreach (var songEntity in songEntities)
			{
				var songModel = songEntity.ToModel();

				var parentDisc = discs[songEntity.DiscId.ToItemId()];
				parentDisc.AddSong(songModel);

				songModel.Artist = songEntity.ArtistId != null ? artists[songEntity.ArtistId.Value.ToItemId()] : null;
				songModel.Genre = songEntity.GenreId != null ? genres[songEntity.GenreId.Value.ToItemId()] : null;

				songModel.Playbacks = songEntity.Playbacks?.Select(x => x.ToModel()).ToList();

				if (!songModel.IsDeleted)
				{
					contentUriProvider.SetSongContentUri(songModel);
				}

				yield return songModel;
			}
		}
	}
}
