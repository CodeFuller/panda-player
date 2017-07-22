using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;

namespace CF.MusicLibrary.AlbumPreprocessor
{
	/// <summary>
	/// Simple wrapper over RelayCommand that accepts task as constructor parameter.
	/// </summary>
	public class AsyncRelayCommand : RelayCommand
	{
		public AsyncRelayCommand(Func<Task> action) :
			base(async () => await action())
		{
		}
	}
}
