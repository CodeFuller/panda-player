using System;
using System.Threading.Tasks;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IEditDiscImageViewModel
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
