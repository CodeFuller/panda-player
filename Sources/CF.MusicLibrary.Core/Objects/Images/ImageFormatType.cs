using System.ComponentModel;

namespace CF.MusicLibrary.Core.Objects.Images
{
	public enum ImageFormatType
	{
		None,

		[Description("JPEG")]
		Jpeg,

		[Description("PNG")]
		Png,

		[Description("Unsupported")]
		Unsupported,
	}
}
