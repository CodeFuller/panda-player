using System;
using System.Text;
using CF.Library.Core.Facades;
using Newtonsoft.Json;
using static CF.Library.Core.Application;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.PersistentPlaylist
{
	public class JsonFilePlaylistDataRepository : IPlaylistDataRepository
	{
		private readonly IFileSystemFacade fileSystemFacade;

		private readonly string dataFileName;

		public JsonFilePlaylistDataRepository(IFileSystemFacade fileSystemFacade, string dataFileName)
		{
			if (fileSystemFacade == null)
			{
				throw new ArgumentNullException(nameof(fileSystemFacade));
			}
			if (dataFileName == null)
			{
				throw new ArgumentNullException(nameof(dataFileName));
			}

			this.fileSystemFacade = fileSystemFacade;
			this.dataFileName = dataFileName;
		}

		public void Save(PlaylistData playlistData)
		{
			var data = JsonConvert.SerializeObject(playlistData, Formatting.Indented);
			fileSystemFacade.WriteAllText(dataFileName, data, Encoding.UTF8);
		}

		public PlaylistData Load()
		{
			if (!fileSystemFacade.FileExists(dataFileName))
			{
				Logger.WriteInfo(Current($"Playlist data file {dataFileName} does not exist."));
				return null;
			}

			var data = fileSystemFacade.ReadAllText(dataFileName, Encoding.UTF8);
			return JsonConvert.DeserializeObject<PlaylistData>(data);
		}

		public void Purge()
		{
			if (fileSystemFacade.FileExists(dataFileName))
			{
				Logger.WriteInfo(Current($"Deleting playlist data file {dataFileName}..."));
				fileSystemFacade.DeleteFile(dataFileName);
			}
		}
	}
}
