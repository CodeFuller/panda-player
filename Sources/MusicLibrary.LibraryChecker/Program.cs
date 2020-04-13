using System.Threading.Tasks;
using CF.Library.Bootstrap;

namespace MusicLibrary.LibraryChecker
{
	public static class Program
	{
		public static async Task<int> Main(string[] args)
		{
			var application = new ConsoleApplication(new ApplicationBootstrapper());
			return await application.Run(args);
		}
	}
}
