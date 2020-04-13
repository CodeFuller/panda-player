using System.ComponentModel;

namespace MusicLibrary.Core.Objects.Images
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
