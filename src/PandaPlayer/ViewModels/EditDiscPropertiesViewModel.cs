﻿using System;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PandaPlayer.Core.Models;
using PandaPlayer.Extensions;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels
{
	public class EditDiscPropertiesViewModel : ViewModelBase, IEditDiscPropertiesViewModel
	{
		private const string ValueForVariousDeleteComments = "<Songs have various delete comments>";

		private readonly IDiscsService discsService;

		private readonly ISongsService songsService;

		private DiscModel Disc { get; set; }

		private string title;

		public bool IsDeleted => Disc.IsDeleted;

		public string Title
		{
			get => title;
			set
			{
				if (String.IsNullOrWhiteSpace(value))
				{
					throw new InvalidOperationException("Value of disc title could not be empty");
				}

				Set(ref title, value);
			}
		}

		private string treeTitle;

		public string TreeTitle
		{
			get => treeTitle;
			set
			{
				if (String.IsNullOrWhiteSpace(value))
				{
					throw new InvalidOperationException("Value of disc tree title could not be empty");
				}

				Set(ref treeTitle, value);
			}
		}

		private string albumTitle;

		public string AlbumTitle
		{
			get => albumTitle;
			set
			{
				if (String.IsNullOrWhiteSpace(value))
				{
					value = null;
				}

				Set(ref albumTitle, value);
			}
		}

		private int? year;

		public int? Year
		{
			get => year;
			set => Set(ref year, value);
		}

		private string deleteComment;

		public string DeleteComment
		{
			get => deleteComment;
			set
			{
				if (String.IsNullOrWhiteSpace(value))
				{
					value = null;
				}

				Set(ref deleteComment, value);
			}
		}

		public EditDiscPropertiesViewModel(IDiscsService discsService, ISongsService songsService)
		{
			this.discsService = discsService ?? throw new ArgumentNullException(nameof(discsService));
			this.songsService = songsService ?? throw new ArgumentNullException(nameof(songsService));
		}

		public void Load(DiscModel disc)
		{
			Disc = disc;
			Title = disc.Title;
			TreeTitle = disc.TreeTitle;
			AlbumTitle = disc.AlbumTitle;
			Year = disc.Year;
			DeleteComment = disc.IsDeleted ? disc.AllSongs.GetDeleteComment(ValueForVariousDeleteComments) : null;
		}

		public async Task Save(CancellationToken cancellationToken)
		{
			Disc.TreeTitle = TreeTitle;
			Disc.Title = Title;
			Disc.AlbumTitle = AlbumTitle;
			Disc.Year = Year;

			if (Disc.IsDeleted && DeleteComment != ValueForVariousDeleteComments)
			{
				foreach (var song in Disc.AllSongs)
				{
					song.DeleteComment = DeleteComment;
					await songsService.UpdateSong(song, cancellationToken);
				}
			}

			await discsService.UpdateDisc(Disc, cancellationToken);
		}
	}
}
