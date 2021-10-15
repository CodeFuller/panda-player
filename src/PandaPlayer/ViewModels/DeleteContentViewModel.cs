using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels
{
	public class DeleteContentViewModel : IDeleteContentViewModel
	{
		private readonly IDiscsService discService;

		private readonly ISongsService songService;

		private Func<CancellationToken, Task> DeleteAction { get; set; }

		public string ConfirmationMessage { get; private set; }

		public string DeleteComment { get; set; }

		public DeleteContentViewModel(IDiscsService discService, ISongsService songService)
		{
			this.discService = discService ?? throw new ArgumentNullException(nameof(discService));
			this.songService = songService ?? throw new ArgumentNullException(nameof(songService));
		}

		public void LoadForSongs(IReadOnlyCollection<SongModel> songs)
		{
			ConfirmationMessage = $"Do you really want to delete {songs.Count} selected song(s)?";
			DeleteComment = null;

			DeleteAction = async cancellationToken =>
			{
				foreach (var song in songs)
				{
					await songService.DeleteSong(song, DeleteComment, cancellationToken);
				}
			};
		}

		public void LoadForDisc(DiscModel disc)
		{
			ConfirmationMessage = $"Do you really want to delete the selected disc '{disc.Title}'?";
			DeleteComment = null;

			DeleteAction = cancellationToken =>
			{
				// We are sending this event to release any disc images hold by DiscImageViewModel.
				Messenger.Default.Send(new LibraryExplorerDiscChangedEventArgs(null));
				return discService.DeleteDisc(disc.Id, DeleteComment, cancellationToken);
			};
		}

		public Task Delete(CancellationToken cancellationToken)
		{
			return DeleteAction(cancellationToken);
		}
	}
}
