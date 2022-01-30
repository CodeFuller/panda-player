using System;
using System.Timers;

namespace PandaPlayer.Facades
{
	internal interface ITimerFacade : IDisposable
	{
		event ElapsedEventHandler Elapsed;

		double Interval { get; set; }

		void Start();

		void Stop();
	}
}
