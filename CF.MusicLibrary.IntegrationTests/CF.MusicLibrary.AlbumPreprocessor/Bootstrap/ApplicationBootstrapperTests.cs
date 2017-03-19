using System;
using CF.MusicLibrary.AlbumPreprocessor.Bootstrap;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels;
using NUnit.Framework;

namespace CF.MusicLibrary.IntegrationTests.CF.MusicLibrary.AlbumPreprocessor.Bootstrap
{
	[TestFixture]
	public class ApplicationBootstrapperTests
	{
		[Test]
		public void Run_RegistersAllDependenciesForRootViewModel()
		{
			//	Arrange

			var target = new ApplicationBootstrapper();

			//	Act

			target.Run();

			//	Assert

			Assert.DoesNotThrow(() => target.GetRootViewModel<MainWindowModel>(String.Empty));
		}
	}
}
