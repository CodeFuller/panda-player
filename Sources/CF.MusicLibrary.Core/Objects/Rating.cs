using System.ComponentModel;

namespace CF.MusicLibrary.Core.Objects
{
	/// <summary>
	/// Enum with possible song ratings.
	/// </summary>
	public enum Rating
	{
		/// <summary>
		/// Invalid rating value that shouldn't happen.
		/// </summary>
		Invalid = 0,

		/// <summary>
		/// Rating of 1 star.
		/// </summary>
		[Description("0.5 stars")]
		R1 = 1,

		/// <summary>
		/// Rating of 2 stars.
		/// </summary>
		[Description("1 star")]
		R2 = 2,

		/// <summary>
		/// Rating of 3 stars.
		/// </summary>
		[Description("1.5 stars")]
		R3 = 3,

		/// <summary>
		/// Rating of 4 stars.
		/// </summary>
		[Description("2 stars")]
		R4 = 4,

		/// <summary>
		/// Rating of 5 stars.
		/// </summary>
		[Description("2.5 stars")]
		R5 = 5,

		/// <summary>
		/// Rating of 6 stars.
		/// </summary>
		[Description("3 stars")]
		R6 = 6,

		/// <summary>
		/// Rating of 7 stars.
		/// </summary>
		[Description("3.5 stars")]
		R7 = 7,

		/// <summary>
		/// Rating of 8 stars.
		/// </summary>
		[Description("4 stars")]
		R8 = 8,

		/// <summary>
		/// Rating of 9 stars.
		/// </summary>
		[Description("4.5 stars")]
		R9 = 9,

		/// <summary>
		/// Rating of 10 stars.
		/// </summary>
		[Description("5 stars")]
		R10 = 10,
	}
}
