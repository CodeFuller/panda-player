using System;
using PandaPlayer.Services.Interfaces;

namespace PandaPlayer.Services
{
	internal class SystemClock : IClock
	{
		public DateTimeOffset Now => DateTimeOffset.Now;
	}
}
