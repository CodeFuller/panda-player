using System.Timers;

namespace MusicLibrary.PandaPlayer.Facades
{
	internal class TimerFacade : Timer, ITimerFacade
	{
		public TimerFacade()
		{
		}

		public TimerFacade(double interval)
			: base(interval)
		{
		}
	}
}
