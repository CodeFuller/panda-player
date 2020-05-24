using System.Collections.Generic;

namespace MusicLibrary.Core.Models
{
#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable
	public class RatingModel
#pragma warning restore CA1052 // Static holder types should be Static or NotInheritable
	{
		public static RatingModel DefaultValue => R5;

		public static RatingModel R1 { get; } = new RatingModel();

		public static RatingModel R2 { get; } = new RatingModel();

		public static RatingModel R3 { get; } = new RatingModel();

		public static RatingModel R4 { get; } = new RatingModel();

		public static RatingModel R5 { get; } = new RatingModel();

		public static RatingModel R6 { get; } = new RatingModel();

		public static RatingModel R7 { get; } = new RatingModel();

		public static RatingModel R8 { get; } = new RatingModel();

		public static RatingModel R9 { get; } = new RatingModel();

		public static RatingModel R10 { get; } = new RatingModel();

		public static IEnumerable<RatingModel> All
		{
			get
			{
				yield return R1;
				yield return R2;
				yield return R3;
				yield return R4;
				yield return R5;
				yield return R6;
				yield return R7;
				yield return R8;
				yield return R9;
				yield return R10;
			}
		}

		private RatingModel()
		{
		}
	}
}
