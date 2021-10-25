﻿using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PandaPlayer.Core.Models;
using PandaPlayer.ViewModels;

namespace PandaPlayer.UnitTests.ViewModels
{
	[TestClass]
	public class SongListItemTests
	{
		[TestMethod]
		public void BitRateGetter_ForActiveSong_ReturnsCorrectValue()
		{
			// Arrange

			var song = new SongModel
			{
				BitRate = 320000,
			};

			var target = new SongListItem(song);

			// Act

			var bitRate = target.BitRate;

			// Assert

			bitRate.Should().Be("320");
		}

		[TestMethod]
		public void BitRateGetter_ForDeletedSong_ReturnsCorrectValue()
		{
			// Arrange

			var song = new SongModel
			{
				BitRate = null,
				DeleteDate = new DateTime(2021, 10, 25),
			};

			var target = new SongListItem(song);

			// Act

			var bitRate = target.BitRate;

			// Assert

			bitRate.Should().Be("N/A");
		}

		[TestMethod]
		public void FileSizeGetter_ForActiveSong_ReturnsCorrectValue()
		{
			// Arrange

			var song = new SongModel
			{
				Size = 123456789,
			};

			var target = new SongListItem(song);

			// Act

			var bitRate = target.FileSize;

			// Assert

			bitRate.Should().Be("117.7 MB");
		}

		[TestMethod]
		public void FileSizeGetter_ForDeletedSong_ReturnsCorrectValue()
		{
			// Arrange

			var song = new SongModel
			{
				Size = null,
				DeleteDate = new DateTime(2021, 10, 25),
			};

			var target = new SongListItem(song);

			// Act

			var bitRate = target.FileSize;

			// Assert

			bitRate.Should().Be("N/A");
		}
	}
}