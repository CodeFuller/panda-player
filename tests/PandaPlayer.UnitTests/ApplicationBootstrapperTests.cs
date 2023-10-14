using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PandaPlayer.ViewModels;

namespace PandaPlayer.UnitTests
{
	[TestClass]
	public class ApplicationBootstrapperTests
	{
		private sealed class TestApplicationBootstrapper : ApplicationBootstrapper
		{
			public T InvokeResolve<T>()
			{
				return Resolve<T>();
			}
		}

		[TestMethod]
		public void RegisterServices_RegistersAllDependenciesForApplicationViewModel()
		{
			// Arrange

			using var target = new TestApplicationBootstrapper();

			// Act

			target.Bootstrap(Array.Empty<string>());

			// Assert

			target.InvokeResolve<ApplicationViewModel>();
		}
	}
}
