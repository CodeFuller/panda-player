using System;
using System.Diagnostics;
using static System.FormattableString;

namespace MusicLibrary.PandaPlayer
{
	public class SystemDefaultWebBrowser : IWebBrowser
	{
		public void OpenPage(string pageAddress)
		{
			pageAddress = pageAddress.Replace("\"", "\\\"", StringComparison.Ordinal);
			Process.Start(Invariant($"\"{pageAddress}\""));
		}
	}
}
