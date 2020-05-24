using System.Collections.Generic;

namespace MusicLibrary.Core.Models
{
	public class RatingModel
	{
		private readonly int value;

		public static RatingModel R1 { get; } = new RatingModel(1);

		public static RatingModel R2 { get; } = new RatingModel(2);

		public static RatingModel R3 { get; } = new RatingModel(3);

		public static RatingModel R4 { get; } = new RatingModel(4);

		public static RatingModel R5 { get; } = new RatingModel(5);

		public static RatingModel R6 { get; } = new RatingModel(6);

		public static RatingModel R7 { get; } = new RatingModel(7);

		public static RatingModel R8 { get; } = new RatingModel(8);

		public static RatingModel R9 { get; } = new RatingModel(9);

		public static RatingModel R10 { get; } = new RatingModel(10);

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

		private RatingModel(int value)
		{
			this.value = value;
		}

		public static RatingModel DefaultValue => R5;
	}
}
