using System;

namespace MusicLibrary.Core.Facades
{
	public interface IClock
	{
		DateTimeOffset Now { get; }
	}
}
