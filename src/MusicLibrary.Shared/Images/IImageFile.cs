using System.ComponentModel;

namespace MusicLibrary.Shared.Images
{
	public interface IImageFile : INotifyPropertyChanged
	{
		string ImageFileName { get; }

		ImageInfo ImageInfo { get; }

		bool ImageIsValid { get; }

		string ImageProperties { get; }

		string ImageStatus { get; }

		void Load(string fileName, bool isTemporaryFile);

		void Unload();
	}
}
