using System.ComponentModel;

namespace PandaPlayer.Shared.Images
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
