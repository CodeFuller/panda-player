using System.ComponentModel;

namespace MusicLibrary.Common.Images
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
