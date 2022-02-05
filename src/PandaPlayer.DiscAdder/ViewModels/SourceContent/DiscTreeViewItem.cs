﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using PandaPlayer.DiscAdder.Events;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal class DiscTreeViewItem : BasicDiscTreeViewItem
	{
		private readonly IReadOnlyCollection<SongTreeViewItem> songItems;

		public override IReadOnlyCollection<BasicDiscTreeViewItem> ChildItems => songItems;

		public override string Title
		{
			get => DiscDirectory;
			set
			{
				DiscDirectory = value;
				RaisePropertyChanged();
			}
		}

		public IEnumerable<SongTreeViewItem> Songs => songItems.Take(songItems.Count - 1);

		public IEnumerable<string> SongFileNames => Songs.Select(s => GetSongFileName(s.Title));

		private string discDirectory;

		public string DiscDirectory
		{
			get => discDirectory;
			set
			{
				// Value has changed or just initialized?
				var valueChanged = discDirectory != null;

				if (valueChanged)
				{
					// Exception, thrown at this point, won't blow up,
					// because exceptions thrown from binding properties are treated as validation failures.
					// http://stackoverflow.com/questions/12658220/exceptions-thrown-during-a-set-operation-in-a-property-are-not-being-caught
					// http://stackoverflow.com/questions/1488472/best-practices-throwing-exceptions-from-properties
					// It's not a big problem because directory title will not be updated and still will be marked as incorrect.
					Directory.Move(discDirectory, value);
				}

				Set(ref discDirectory, value);

				if (valueChanged)
				{
					OnDiscContentChanged();
				}
			}
		}

		private static void OnDiscContentChanged()
		{
			Messenger.Default.Send(new DiscContentChangedEventArgs());
		}

		private bool contentIsIncorrect;

		public bool ContentIsIncorrect
		{
			get => contentIsIncorrect;
			set => Set(ref contentIsIncorrect, value);
		}

		public DiscTreeViewItem(DiscContent disc)
		{
			if (disc == null)
			{
				throw new ArgumentNullException(nameof(disc));
			}

			DiscDirectory = disc.DiscDirectory;

			var songSeparator = new SongTreeViewItem(new SongContent(String.Empty));
			songItems = new ReadOnlyCollection<SongTreeViewItem>(disc.Songs.Select(CreateSongItem).Concat(Enumerable.Repeat(songSeparator, 1)).ToList());
		}

		private SongTreeViewItem CreateSongItem(SongContent songContent)
		{
			var songItem = new SongTreeViewItem(songContent);
			songItem.SongTitleChanging += SongTitle_Changing;
			songItem.SongTitleChanged += SongTitle_Changed;

			return songItem;
		}

		private void SongTitle_Changing(object sender, SongTitleChangingEventArgs e)
		{
			if (e.OldTitle != null)
			{
				File.Move(GetSongFileName(e.OldTitle), GetSongFileName(e.NewTitle));
			}
		}

		private static void SongTitle_Changed(object sender, SongTitleChangedEventArgs e)
		{
			if (e.OldTitle != null)
			{
				OnDiscContentChanged();
			}
		}

		private string GetSongFileName(string songTitle)
		{
			return Path.Combine(DiscDirectory, songTitle);
		}
	}
}
