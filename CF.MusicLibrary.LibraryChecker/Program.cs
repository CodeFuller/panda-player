using System.Threading.Tasks;
using CF.Library.Bootstrap;

namespace CF.MusicLibrary.LibraryChecker
{
	public static class Program
	{
		static async Task<int> Main(string[] args)
		{
			var application = new ConsoleApplication(new ApplicationBootstrapper());
			return await application.Run(args);
		}
	}
}
