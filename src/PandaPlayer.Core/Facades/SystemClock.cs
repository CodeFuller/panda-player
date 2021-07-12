using System;

namespace PandaPlayer.Core.Facades
{
	public class SystemClock : IClock
	{
		public DateTimeOffset Now => DateTimeOffset.Now;
	}
}
