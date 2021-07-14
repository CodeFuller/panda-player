using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using PandaPlayer.Core.Models;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface IEditDiscImageViewModel
	{
		DiscModel Disc { get; }

		string ImageFileName { get; }

		bool ImageIsValid { get; }

		bool ImageWasChanged { get; }

		string ImageProperties { get; }

		string ImageStatus { get; }

		public ICommand LaunchSearchForDiscImageCommand { get; }

		Task SetImage(Uri imageUri);

		void SetImage(byte[] imageData);

		void Load(DiscModel disc);

		void Unload();

		Task Save(CancellationToken cancellationToken);
	}
}
