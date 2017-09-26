﻿using System;
using System.ComponentModel;

namespace CF.MusicLibrary.Universal.DiscArt
{
	[Flags]
	public enum DiscArtValidationResults
	{
		None,

		[Description("Image is OK")]
		ImageIsOk = 0x01,

		[Description("Image is too small")]
		ImageIsTooSmall = 0x02,

		[Description("Image is too big")]
		ImageIsTooBig = 0x04,

		[Description("Image file is too big")]
		FileSizeIsTooBig = 0x08,

		[Description("Unsupported format")]
		UnsupportedFormat = 0x10,
	}
}
