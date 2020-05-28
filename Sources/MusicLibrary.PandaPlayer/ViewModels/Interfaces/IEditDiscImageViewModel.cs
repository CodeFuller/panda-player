using System;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;

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

		void Load(DiscModel disc);

		void Unload();

		Task Save();
	}
}
