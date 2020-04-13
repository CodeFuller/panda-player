using System.ComponentModel;
using MusicLibrary.Core.Objects.Images;

namespace MusicLibrary.Common.Images
{
	public interface IImageFile : INotifyPropertyChanged
	{
		string ImageFileName { get; }

		bool IsTemporaryFile { get; }

		ImageInfo ImageInfo { get; }

		bool ImageIsValid { get; }

		string ImageProperties { get; }

		string ImageStatus { get; }

		void Load(string fileName, bool isTemporaryFile);

		void Unload();
	}
}
