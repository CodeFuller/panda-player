using System;
using CF.Library.Core.Facades;
using Force.Crc32;
using MusicLibrary.Dal.LocalDb.Interfaces;

namespace MusicLibrary.Dal.LocalDb.Internal
{
	internal class Crc32Calculator : IChecksumCalculator
	{
		private readonly IFileSystemFacade fileSystemFacade;

		public Crc32Calculator(IFileSystemFacade fileSystemFacade)
		{
			this.fileSystemFacade = fileSystemFacade ?? throw new ArgumentNullException(nameof(fileSystemFacade));
		}

		public uint CalculateChecksum(string fileName)
		{
			var data = fileSystemFacade.ReadAllBytes(fileName);
			return Crc32Algorithm.Compute(data);
		}
	}
}
