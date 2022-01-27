using System.Collections.Generic;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.Internal
{
	internal interface IDiscLibrary
	{
		IReadOnlyCollection<FolderModel> Folders { get; }

		IReadOnlyCollection<DiscModel> Discs { get; }

		IReadOnlyCollection<ArtistModel> Artists { get; }

		IReadOnlyCollection<GenreModel> Genres { get; }

		IReadOnlyCollection<AdviseGroupModel> AdviseGroups { get; }

		IReadOnlyCollection<AdviseSetModel> AdviseSets { get; }

		void AddEmptyFolder(ShallowFolderModel folder);

		FolderModel GetFolder(ItemId folderId);

		void AddDisc(DiscModel disc);

		DiscModel GetDisc(ItemId discId);

		void AddSong(SongModel song);

		IReadOnlyCollection<SongModel> TryGetSongs(IEnumerable<ItemId> songIds);

		void AddArtist(ArtistModel artist);

		void AddAdviseGroup(AdviseGroupModel adviseGroup);

		void DeleteAdviseGroup(AdviseGroupModel adviseGroup);

		void AddAdviseSet(AdviseSetModel adviseSet);

		void DeleteAdviseSet(AdviseSetModel adviseSet);

		void AddDiscImage(DiscImageModel discImage);

		void DeleteDiscImage(DiscImageModel discImage);
	}
}
