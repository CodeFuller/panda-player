using System;
using Force.Crc32;
using PandaPlayer.Core.Facades;
using PandaPlayer.Dal.LocalDb.Interfaces;

namespace PandaPlayer.Dal.LocalDb.Internal
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
