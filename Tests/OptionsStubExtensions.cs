using Microsoft.Extensions.Options;
using NSubstitute;

namespace CF.MusicLibrary.Tests
{
	public static class OptionsStubExtensions
	{
		public static IOptions<TSettings> StubOptions<TSettings>(this TSettings settings) where TSettings : class, new()
		{
			IOptions<TSettings> options = Substitute.For<IOptions<TSettings>>();
			options.Value.Returns(settings);
			return options;
		}
	}
}
