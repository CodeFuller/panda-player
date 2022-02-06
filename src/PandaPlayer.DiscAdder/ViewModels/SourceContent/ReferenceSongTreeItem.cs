using System;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal class ReferenceSongTreeItem : ReferenceBasicTreeItem
	{
		private ReferenceSongContent song;

		private ReferenceSongContent Song
		{
			get => song;
			set
			{
				song = value;
				RaisePropertyChanged(nameof(Title));
			}
		}

		public string ExpectedTitle => Song.ExpectedTitle;

		public string ExpectedTitleWithTrackNumber => Song.ExpectedTitleWithTrackNumber;

		public override string Title => ExpectedTitle;

		public ReferenceSongTreeItem(ReferenceSongContent song)
		{
			Song = song ?? throw new ArgumentNullException(nameof(song));
		}

		public void Update(ReferenceSongContent newSongContent)
		{
			Song = newSongContent;
		}
	}
}
