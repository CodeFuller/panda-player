using System;
using System.Timers;

namespace PandaPlayer.Facades
{
	internal interface ITimerFacade : IDisposable
	{
		bool Enabled { get; set; }

		event ElapsedEventHandler Elapsed;

		double Interval { get; set; }

		bool AutoReset { get; set; }

		void Start();

		void Stop();
	}
}
