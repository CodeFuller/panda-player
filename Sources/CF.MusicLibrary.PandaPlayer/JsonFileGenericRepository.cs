using System;
using System.Text;
using CF.Library.Core.Facades;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.PandaPlayer
{
	public class JsonFileGenericRepository<T> : IGenericDataRepository<T>
		where T : class
	{
		private readonly IFileSystemFacade fileSystemFacade;
		private readonly ILogger<JsonFileGenericRepository<T>> logger;
		private readonly string dataFileName;

		public JsonFileGenericRepository(IFileSystemFacade fileSystemFacade, ILogger<JsonFileGenericRepository<T>> logger, string dataFileName)
		{
			this.fileSystemFacade = fileSystemFacade ?? throw new ArgumentNullException(nameof(fileSystemFacade));
			this.dataFileName = dataFileName ?? throw new ArgumentNullException(nameof(dataFileName));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
				logger.LogInformation(Current($"Data file {dataFileName} does not exist."));
				return default(T);
			}

			var data = fileSystemFacade.ReadAllText(dataFileName, Encoding.UTF8);
			return JsonConvert.DeserializeObject<T>(data);
		}

		public void Purge()
		{
			if (fileSystemFacade.FileExists(dataFileName))
			{
				logger.LogInformation(Current($"Deleting data file {dataFileName}..."));
				fileSystemFacade.DeleteFile(dataFileName);
			}
		}
	}
}
