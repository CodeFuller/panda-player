namespace MusicLibrary.Dal.LocalDb.Interfaces
{
	internal interface IChecksumCalculator
	{
		uint CalculateChecksum(string fileName);
	}
}
