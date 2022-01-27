using System;
using System.Collections.Generic;
using System.Linq;
using PandaPlayer.Core.Comparers;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.Internal
{
	internal class DiscLibrary : IDiscLibrary
	{
		private readonly Dictionary<ItemId, FolderModel> folders;

		private readonly Dictionary<ItemId, DiscModel> discs;

		private readonly Dictionary<ItemId, SongModel> songs;

		private readonly Dictionary<ItemId, ArtistModel> artists;

		private readonly Dictionary<ItemId, GenreModel> genres;

		private readonly Dictionary<ItemId, AdviseGroupModel> adviseGroups;

		private readonly Dictionary<ItemId, AdviseSetModel> adviseSets;

		// The only reason for sorting folders is strict ordering check in IT.
		public IReadOnlyCollection<FolderModel> Folders => folders.Values.OrderBy(x => x.Id.Value).ToList();

		// The only reason for sorting discs is strict ordering check in IT.
		public IReadOnlyCollection<DiscModel> Discs => discs.Values.OrderBy(x => x.Id.Value).ToList();

		public IReadOnlyCollection<ArtistModel> Artists => artists.Values.OrderBy(x => x.Name).ToList();

		public IReadOnlyCollection<GenreModel> Genres => genres.Values.OrderBy(x => x.Name).ToList();

		public IReadOnlyCollection<AdviseGroupModel> AdviseGroups => adviseGroups.Values.OrderBy(x => x.Name).ToList();

		public IReadOnlyCollection<AdviseSetModel> AdviseSets => adviseSets.Values.OrderBy(x => x.Name).ToList();

		public DiscLibrary(IReadOnlyCollection<DiscModel> discs, IEnumerable<ShallowFolderModel> foldersWithoutDiscs, IEnumerable<ArtistModel> emptyArtists,
			IEnumerable<GenreModel> emptyGenres, IEnumerable<AdviseGroupModel> emptyAdviseGroups, IEnumerable<AdviseSetModel> emptyAdviseSets)
		{
			this.discs = discs.ToDictionary(x => x.Id, x => x);
			this.songs = discs.SelectMany(x => x.AllSongs).ToDictionary(x => x.Id, x => x);

			this.folders = CreateFolders(discs, foldersWithoutDiscs);

			this.artists = songs.Values
				.Select(x => x.Artist)
				.Where(x => x != null)
				.Concat(emptyArtists)
				.Distinct(new ArtistEqualityComparer())
				.ToDictionary(x => x.Id, x => x);

			this.genres = songs.Values
				.Select(x => x.Genre)
				.Where(x => x != null)
				.Concat(emptyGenres)
				.Distinct(new GenreEqualityComparer())
				.ToDictionary(x => x.Id, x => x);

			this.adviseGroups = folders.Values.Select(x => x.AdviseGroup)
				.Concat(discs.Select(x => x.AdviseGroup))
				.Where(x => x != null)
				.Concat(emptyAdviseGroups)
				.Distinct(new AdviseGroupEqualityComparer())
				.ToDictionary(x => x.Id, x => x);

			this.adviseSets = discs.Select(x => x.AdviseSetInfo?.AdviseSet)
				.Where(x => x != null)
				.Concat(emptyAdviseSets)
				.Distinct(new AdviseSetEqualityComparer())
				.ToDictionary(x => x.Id, x => x);
		}

		private static Dictionary<ItemId, FolderModel> CreateFolders(IReadOnlyCollection<DiscModel> discs, IEnumerable<ShallowFolderModel> foldersWithoutDiscs)
		{
			var discFolders = discs
				.Select(x => x.Folder)
				.Distinct(new ShallowFolderEqualityComparer());

			var folders = discFolders.Concat(foldersWithoutDiscs)
				.ToDictionary(x => x.Id, x => new FolderModel
				{
					Id = x.Id,
					ParentFolderId = x.ParentFolderId,
					Name = x.Name,
					AdviseGroup = x.AdviseGroup,
					DeleteDate = x.DeleteDate,
				});

			LinkFolders(folders, discs);

			return folders;
		}

		private static void LinkFolders(Dictionary<ItemId, FolderModel> folders, IReadOnlyCollection<DiscModel> discs)
		{
			var childFolders = folders.Values
				.Where(x => !x.IsRoot)
				.GroupBy(x => x.ParentFolderId)
				.ToDictionary(x => x.Key, x => x.ToList());

			var childDiscs = discs
				.GroupBy(x => x.Folder.Id)
				.ToDictionary(x => x.Key, x => x.ToList());

			foreach (var folder in folders.Values)
			{
				folder.ParentFolder = folder.IsRoot ? null : folders[folder.ParentFolderId];
				folder.Subfolders = childFolders.TryGetValue(folder.Id, out var subfolders) ? subfolders : new List<FolderModel>();
				folder.Discs = childDiscs.TryGetValue(folder.Id, out var folderDiscs) ? folderDiscs : new List<DiscModel>();
			}

			foreach (var disc in discs)
			{
				disc.Folder = folders[disc.Folder.Id];
			}
		}

		public void AddEmptyFolder(ShallowFolderModel folder)
		{
			var newFolder = new FolderModel
			{
				Id = folder.Id,
				Name = folder.Name,
				ParentFolderId = folder.ParentFolderId,
				ParentFolder = GetFolder(folder.ParentFolderId),
				Subfolders = new List<ShallowFolderModel>(),
				Discs = new List<DiscModel>(),
			};

			folders.Add(newFolder.Id, newFolder);

			GetFolder(folder.ParentFolderId).AddSubfolder(newFolder);
		}

		public FolderModel GetFolder(ItemId folderId)
		{
			if (!folders.TryGetValue(folderId, out var folder))
			{
				throw new InvalidOperationException($"Folder '{folderId}' was not found");
			}

			return folder;
		}

		public void AddDisc(DiscModel disc)
		{
			discs.Add(disc.Id, disc);
		}

		public DiscModel GetDisc(ItemId discId)
		{
			if (!discs.TryGetValue(discId, out var disc))
			{
				throw new InvalidOperationException($"Disc '{discId}' was not found");
			}

			return disc;
		}

		public void AddSong(SongModel song)
		{
			songs.Add(song.Id, song);
		}

		public IReadOnlyCollection<SongModel> TryGetSongs(IEnumerable<ItemId> songIds)
		{
			var foundSongs = new List<SongModel>();

			foreach (var songId in songIds)
			{
				if (songs.TryGetValue(songId, out var song))
				{
					foundSongs.Add(song);
				}
			}

			return foundSongs;
		}

		public void AddArtist(ArtistModel artist)
		{
			artists.Add(artist.Id, artist);
		}

		public void AddAdviseGroup(AdviseGroupModel adviseGroup)
		{
			adviseGroups.Add(adviseGroup.Id, adviseGroup);
		}

		public void DeleteAdviseGroup(AdviseGroupModel adviseGroup)
		{
			adviseGroups.Remove(adviseGroup.Id);
		}

		public void AddAdviseSet(AdviseSetModel adviseSet)
		{
			adviseSets.Add(adviseSet.Id, adviseSet);
		}

		public void DeleteAdviseSet(AdviseSetModel adviseSet)
		{
			adviseSets.Remove(adviseSet.Id);
		}

		public void AddDiscImage(DiscImageModel discImage)
		{
			// Doing nothing here since collection of disc images is not maintained.
		}

		public void DeleteDiscImage(DiscImageModel discImage)
		{
			// Doing nothing here since collection of disc images is not maintained.
		}
	}
}
