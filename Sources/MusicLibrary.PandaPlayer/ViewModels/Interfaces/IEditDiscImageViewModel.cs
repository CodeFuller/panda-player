using System;
using System.Threading.Tasks;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IEditDiscImageViewModel
	{
		DiscModel Disc { get; }

		string ImageFileName { get; }

		bool ImageIsValid { get; }

		bool ImageWasChanged { get; }

		string ImageProperties { get; }

		string ImageStatus { get; }

		Task SetImage(Uri imageUri);

		void SetImage(byte[] imageData);

		Task Load(DiscModel disc);

		void Unload();

		Task Save();
	}
}
