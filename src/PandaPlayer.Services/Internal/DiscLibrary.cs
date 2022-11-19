using System;
using System.Collections.Generic;
using System.Linq;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.Internal
{
	public class DiscLibrary : IDiscLibrary
	{
		private readonly Dictionary<ItemId, FolderModel> folders;

		private readonly Dictionary<ItemId, DiscModel> discs;

		private readonly Dictionary<ItemId, SongModel> songs;

		private readonly Dictionary<ItemId, ArtistModel> artists;

		private readonly Dictionary<ItemId, GenreModel> genres;

		private readonly Dictionary<ItemId, AdviseGroupModel> adviseGroups;

		private readonly Dictionary<ItemId, AdviseSetModel> adviseSets;

		public IReadOnlyCollection<FolderModel> Folders => folders.Values.ToList();

		// The only reason for sorting discs is strict ordering check in IT.
		public IReadOnlyCollection<DiscModel> Discs => discs.Values.OrderBy(x => x.Id.Value).ToList();

		public IReadOnlyCollection<ArtistModel> Artists => artists.Values.OrderBy(x => x.Name).ToList();

		public IReadOnlyCollection<GenreModel> Genres => genres.Values.OrderBy(x => x.Name).ToList();

		public IReadOnlyCollection<AdviseGroupModel> AdviseGroups => adviseGroups.Values.OrderBy(x => x.Name).ToList();

		public IReadOnlyCollection<AdviseSetModel> AdviseSets => adviseSets.Values.OrderBy(x => x.Name).ToList();

		public DiscLibrary(IEnumerable<FolderModel> folders, IEnumerable<DiscModel> discs, IEnumerable<SongModel> songs,
			IEnumerable<ArtistModel> artists, IEnumerable<GenreModel> genres, IEnumerable<AdviseGroupModel> adviseGroups, IEnumerable<AdviseSetModel> adviseSets)
		{
			this.folders = folders?.ToDictionary(x => x.Id, x => x) ?? throw new ArgumentNullException(nameof(folders));
			this.discs = discs?.ToDictionary(x => x.Id, x => x) ?? throw new ArgumentNullException(nameof(discs));
			this.songs = songs?.ToDictionary(x => x.Id, x => x) ?? throw new ArgumentNullException(nameof(songs));
			this.artists = artists?.ToDictionary(x => x.Id, x => x) ?? throw new ArgumentNullException(nameof(artists));
			this.genres = genres?.ToDictionary(x => x.Id, x => x) ?? throw new ArgumentNullException(nameof(genres));
			this.adviseGroups = adviseGroups?.ToDictionary(x => x.Id, x => x) ?? throw new ArgumentNullException(nameof(adviseGroups));
			this.adviseSets = adviseSets?.ToDictionary(x => x.Id, x => x) ?? throw new ArgumentNullException(nameof(adviseSets));
		}

		public void AddFolder(FolderModel folder)
		{
			folders.Add(folder.Id, folder);
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
