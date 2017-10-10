namespace CF.MusicLibrary.BL.Interfaces
{
	public interface IDiscArtFileStorage
	{
		string GetDiscCoverImageFileName(string discDirectory);

		void StoreDiscCoverImage(string discDirectory, string sourceCoverImageFileName);

		bool IsCoverImageFile(string filePath);
	}
}
