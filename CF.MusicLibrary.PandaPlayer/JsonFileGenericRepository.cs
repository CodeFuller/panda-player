using System;
using System.Text;
using CF.Library.Core.Facades;
using Newtonsoft.Json;
using static CF.Library.Core.Application;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.PandaPlayer
{
	public class JsonFileGenericRepository<T> : IGenericDataRepository<T> where T : class
	{
		private readonly IFileSystemFacade fileSystemFacade;

		private readonly string dataFileName;

		public JsonFileGenericRepository(IFileSystemFacade fileSystemFacade, string dataFileName)
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

		public void Save(T data)
		{
			var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
			fileSystemFacade.WriteAllText(dataFileName, jsonData, Encoding.UTF8);
		}

		public T Load()
		{
			if (!fileSystemFacade.FileExists(dataFileName))
			{
				Logger.WriteInfo(Current($"Data file {dataFileName} does not exist."));
				return default(T);
			}

			var data = fileSystemFacade.ReadAllText(dataFileName, Encoding.UTF8);
			return JsonConvert.DeserializeObject<T>(data);
		}

		public void Purge()
		{
			if (fileSystemFacade.FileExists(dataFileName))
			{
				Logger.WriteInfo(Current($"Deleting data file {dataFileName}..."));
				fileSystemFacade.DeleteFile(dataFileName);
			}
		}
	}
}
