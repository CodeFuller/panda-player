using System;

namespace MusicLibrary.Core.Facades
{
	public class SystemClock : IClock
	{
		public DateTimeOffset Now => DateTimeOffset.Now;
	}
}
