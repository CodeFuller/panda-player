using System.Diagnostics;
using static System.FormattableString;

namespace CF.MusicLibrary.PandaPlayer
{
	public class SystemDefaultWebBrowser : IWebBrowser
	{
		public void OpenPage(string pageAddress)
		{
			pageAddress = pageAddress.Replace("\"", "\\\"");
			Process.Start(Invariant($"\"{pageAddress}\""));
		}
	}
}
