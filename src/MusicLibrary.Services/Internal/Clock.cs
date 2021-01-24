using System;
using MusicLibrary.Services.Interfaces;

namespace MusicLibrary.Services.Internal
{
	internal class Clock : IClock
	{
		public DateTimeOffset Now => DateTimeOffset.Now;
	}
}
