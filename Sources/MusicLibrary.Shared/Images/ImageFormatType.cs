using System.ComponentModel;

namespace MusicLibrary.Shared.Images
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
