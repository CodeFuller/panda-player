using System;
using NUnit.Framework;

namespace CF.MusicLibrary.LibraryToolkit.Tests
{
	[TestFixture]
	public class ApplicationBootstrapperTests
	{
		[Test]
		public void RegisterDependencies_RegistersAllDependenciesForApplicationLogic()
		{
			// Arrange

			var target = new ApplicationBootstrapper();

			// Act & Assert

			Assert.DoesNotThrow(() => target.Bootstrap(Array.Empty<string>()));
		}
	}
}
