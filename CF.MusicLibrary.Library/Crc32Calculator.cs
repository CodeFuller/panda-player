using System;
using CF.Library.Core.Facades;
using Force.Crc32;

namespace CF.MusicLibrary.Library
{
	public class Crc32Calculator : IChecksumCalculator
	{
		private readonly IFileSystemFacade fileSystemFacade;

		public Crc32Calculator(IFileSystemFacade fileSystemFacade)
		{
			if (fileSystemFacade == null)
			{
				throw new ArgumentNullException(nameof(fileSystemFacade));
			}

			this.fileSystemFacade = fileSystemFacade;
		}

		public int CalculateChecksumForFile(string fileName)
		{
			var data = fileSystemFacade.ReadAllBytes(fileName);
			return (int)Crc32Algorithm.Compute(data);
		}
	}
}
