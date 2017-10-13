namespace CF.MusicLibrary.Core.Interfaces
{
	public interface IDiscArtFileStorage
	{
		string GetDiscCoverImageFileName(string discDirectory);

		void StoreDiscCoverImage(string discDirectory, string sourceCoverImageFileName);

		bool IsCoverImageFile(string filePath);
	}
}
