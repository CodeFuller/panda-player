﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FluentAssertions;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Events.SongListEvents;
using MusicLibrary.PandaPlayer.ViewModels;

namespace MusicLibrary.PandaPlayer.UnitTests.Extensions
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
				nameof(SongPlaylistViewModel.HasSongs),
				nameof(SongPlaylistViewModel.SongsNumber),
				nameof(SongPlaylistViewModel.TotalSongsFileSize),
				nameof(SongPlaylistViewModel.TotalSongsDuration),
			};

			events.Select(e => e.PropertyName).Should().BeEquivalentTo(expectedProperties);
		}
	}
}
