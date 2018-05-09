using System;
using System.Collections.Generic;
using System.Linq;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public static class RatingsHelper
	{
		public static IEnumerable<Rating> AllowedRatingsDesc => Enum.GetValues(typeof(Rating))
			.Cast<Rating>().Where(r => r != Rating.Invalid).OrderByDescending(r => r);
	}
}
