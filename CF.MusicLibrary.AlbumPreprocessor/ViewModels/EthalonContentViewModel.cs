using System;
using System.IO;
using CF.Library.Core.Facades;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels
{
	public class EthalonContentViewModel : ViewModelBase
	{
		private const string ContentSaveFilename = "RawAlbumsContent.txt";

		private string rawEthalonAlbumsContent;

		private readonly IFileSystemFacade fileSystemFacade;
		private readonly string contentSaveFilePath;

		public EthalonContentViewModel(IFileSystemFacade fileSystemFacade)
		{
			if (fileSystemFacade == null)
			{
				throw  new ArgumentNullException(nameof(fileSystemFacade));
			}

			this.fileSystemFacade = fileSystemFacade;
			contentSaveFilePath = Path.Combine(fileSystemFacade.GetProcessDirectory(), ContentSaveFilename);
		}

		public string Content
		{
			get { return rawEthalonAlbumsContent; }
			set
			{
				Set(ref rawEthalonAlbumsContent, value);
				SaveRawEthalonAlbumsContent();
			}
		}

		public void LoadRawEthalonAlbumsContent()
		{
			if (fileSystemFacade.FileExists(contentSaveFilePath))
			{
				Content = File.ReadAllText(contentSaveFilePath);
			}
		}

		private void SaveRawEthalonAlbumsContent()
		{
			File.WriteAllText(contentSaveFilePath, Content);
		}
	}
}
