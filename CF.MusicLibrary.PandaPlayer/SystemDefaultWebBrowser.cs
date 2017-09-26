using System;
using System.Diagnostics;

namespace CF.MusicLibrary.PandaPlayer
{
	public class SystemDefaultWebBrowser : IWebBrowser
	{
		public void OpenPage(Uri pageUri)
		{
			var pageAddress = Uri.EscapeUriString(pageUri.ToString());
			Process.Start(pageAddress);
		}
	}
}
