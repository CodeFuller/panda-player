using System;
using System.Diagnostics;

namespace PandaPlayer
{
	public class SystemDefaultWebBrowser : IWebBrowser
	{
		public void OpenPage(string pageAddress)
		{
			pageAddress = pageAddress.Replace("\"", "\\\"", StringComparison.Ordinal);

			var psi = new ProcessStartInfo
			{
				FileName = pageAddress,
				UseShellExecute = true,
			};

			Process.Start(psi);
		}
	}
}
