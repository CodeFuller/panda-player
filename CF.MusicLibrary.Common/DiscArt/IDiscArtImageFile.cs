using System.ComponentModel;

namespace CF.MusicLibrary.Common.DiscArt
{
	public interface IDiscArtImageFile : INotifyPropertyChanged
	{
		string ImageFileName { get; }

		bool IsTemporaryFile { get; }

		DiscArtImageInfo ImageInfo { get; }

		bool ImageIsValid { get; }

		string ImageProperties { get; }

		string ImageStatus { get; }

		void Load(string fileName, bool isTemporaryFile);

		void Unload();
	}
}
