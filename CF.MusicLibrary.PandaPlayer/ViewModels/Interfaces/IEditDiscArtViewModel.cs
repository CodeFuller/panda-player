using System;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IEditDiscArtViewModel
	{
		Disc Disc { get; }

		string ImageFileName { get; }

		bool ImageIsValid { get; }

		bool ImageWasChanged { get; }

		string ImageProperties { get; }

		string ImageStatus { get; }

		Task SetImage(Uri imageUri);

		void SetImage(byte[] imageData);

		Task Load(Disc disc);

		void Unload();

		Task Save();
	}
}
