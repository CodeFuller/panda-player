using System.Threading.Tasks;
using CF.Library.Bootstrap;

namespace CF.MusicLibrary.LibraryToolkit
{
	public static class Program
	{
		public static async Task Main(string[] args)
		{
			var application = new ConsoleApplication(new ApplicationBootstrapper());
			await application.Run(args);
		}
	}
}
