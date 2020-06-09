﻿using System;
using System.IO;
using CF.Library.Core.Facades;
using GalaSoft.MvvmLight;

namespace MusicLibrary.DiscAdder.ViewModels.SourceContent
{
	internal class ReferenceContentViewModel : ViewModelBase
	{
		// TODO: Store this info in the database
		private const string ContentSaveFilename = "RawDiscsContent.txt";

		private readonly IFileSystemFacade fileSystemFacade;
		private readonly string contentSaveFilePath;

		public ReferenceContentViewModel(IFileSystemFacade fileSystemFacade, string appDataPath)
		{
			this.fileSystemFacade = fileSystemFacade ?? throw new ArgumentNullException(nameof(fileSystemFacade));
			contentSaveFilePath = Path.Combine(appDataPath, ContentSaveFilename);
		}

		private string rawEthalonDiscsContent;

		public string Content
		{
			get => rawEthalonDiscsContent;
			set
			{
				Set(ref rawEthalonDiscsContent, value);
				SaveRawEthalonDiscsContent();
			}
		}

		public void LoadRawEthalonDiscsContent()
		{
			if (fileSystemFacade.FileExists(contentSaveFilePath))
			{
				Content = File.ReadAllText(contentSaveFilePath);
			}
		}

		private void SaveRawEthalonDiscsContent()
		{
			File.WriteAllText(contentSaveFilePath, Content);
		}
	}
}