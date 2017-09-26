﻿using System.Windows;
using System.Windows.Media.Imaging;

namespace CF.MusicLibrary.PandaPlayer.Views.ClipboardAccess
{
	class ClipboardDataProvider : IClipboardDataProvider
	{
		private readonly ClipboardImageExtractor clipboardImageExtractor = new ClipboardImageExtractor();

		public string GetTextData()
		{
			return Clipboard.ContainsText() ? Clipboard.GetText() : null;
		}

		public BitmapFrame GetImageData()
		{
			return Clipboard.ContainsImage() ? clipboardImageExtractor.GetImageFromClipboard() : null;
		}
	}
}
