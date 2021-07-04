using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MusicLibrary.Services.Diagnostic;
using MusicLibrary.Services.Diagnostic.Inconsistencies;
using MusicLibrary.Services.Extensions;
using MusicLibrary.Services.Interfaces;

namespace MusicLibrary.Services.IntegrationTests
{
	[TestClass]
	public class DiagnosticServiceTests : BasicServiceTests<IDiagnosticService>
	{
		[TestMethod]
		public async Task CheckLibrary_ForValidLibrary_DoesNotRegisterAnyInconsistencies()
		{
			// Arrange

			var inconsistencies = new List<LibraryInconsistency>();
			void InconsistenciesHandler(LibraryInconsistency inconsistency) => inconsistencies.Add(inconsistency);

			var target = CreateTestTarget(services =>
			{
				services.AddDiscTitleToAlbumMapper(settings =>
				{
					settings.AlbumTitlePatterns = new[]
					{
						@"^(.+) \(CD ?\d+\)$",
					};

					settings.EmptyAlbumTitlePatterns = new[]
					{
						"Disc With Missing Fields",
					};
				});
			});

			// Act

			await target.CheckLibrary(LibraryCheckFlags.All, Mock.Of<IOperationProgress>(), InconsistenciesHandler, CancellationToken.None);

			// Assert

			inconsistencies.Should().BeEmpty();
		}
	}
}
