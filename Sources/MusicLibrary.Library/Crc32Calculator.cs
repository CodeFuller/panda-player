using System;
using CF.Library.Core.Facades;
using Force.Crc32;

namespace MusicLibrary.Library
{
	public class Crc32Calculator : IChecksumCalculator
	{
		private readonly IFileSystemFacade fileSystemFacade;

		public Crc32Calculator(IFileSystemFacade fileSystemFacade)
		{
			this.fileSystemFacade = fileSystemFacade ?? throw new ArgumentNullException(nameof(fileSystemFacade));
		}

		public int CalculateChecksumForFile(string fileName)
		{
			var data = fileSystemFacade.ReadAllBytes(fileName);
			return (int)Crc32Algorithm.Compute(data);
		}
	}
}
