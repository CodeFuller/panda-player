using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FluentAssertions;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.SongListEvents;
using PandaPlayer.ViewModels;

namespace PandaPlayer.UnitTests.Extensions
{
	public static class EventArgsExtensions
	{
		public static void RegisterEvent<TEventArgs>(this TEventArgs inputEvent, ref TEventArgs outputEvent)
			where TEventArgs : EventArgs
		{
			if (outputEvent != null)
			{
				throw new InvalidOperationException("Event was sent twice unexpectedly");
			}

			outputEvent = inputEvent;
		}

		public static void VerifyPlaylistEvent(this BasicPlaylistEventArgs playlistEvent, IReadOnlyList<SongModel> expectedSongs, int? expectedCurrentSongIndex)
		{
			playlistEvent.Should().NotBeNull();
			playlistEvent.Songs.Should().BeEquivalentTo(expectedSongs, x => x.WithStrictOrdering());
			playlistEvent.CurrentSong.Should().Be(expectedCurrentSongIndex == null ? null : expectedSongs[expectedCurrentSongIndex.Value]);
			playlistEvent.CurrentSongIndex.Should().Be(expectedCurrentSongIndex);
		}

		public static void VerifySongListPropertyChangedEvents(this IEnumerable<PropertyChangedEventArgs> events)
		{
			var expectedProperties = new[]
			{
				nameof(PlaylistViewModel.HasSongs),
				nameof(PlaylistViewModel.SongsNumber),
				nameof(PlaylistViewModel.TotalSongsFileSize),
				nameof(PlaylistViewModel.TotalSongsDuration),
			};

			events.Select(e => e.PropertyName).Should().BeEquivalentTo(expectedProperties);
		}
	}
}
