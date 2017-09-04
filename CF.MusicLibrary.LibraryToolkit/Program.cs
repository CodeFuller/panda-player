using CF.Library.Core.Bootstrap;

namespace CF.MusicLibrary.LibraryToolkit
{
	public static class Program
	{
		/// <summary>
		/// Property Injection for Console Application.
		/// </summary>
		public static IConsoleApplication Application { get; set; } = new ConsoleApplication(new Bootstrapper());

		static void Main(string[] args)
		{
			Application.Run(args);
		}
	}
}
