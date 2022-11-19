using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Services.Interfaces.Dal;

namespace PandaPlayer.Services.Internal
{
	internal class DiscLibraryInitializer : IApplicationInitializer
	{
		private readonly IDiscLibraryRepository discLibraryRepository;

		private readonly ILogger<DiscLibraryInitializer> logger;

		public DiscLibraryInitializer(IDiscLibraryRepository discLibraryRepository, ILogger<DiscLibraryInitializer> logger)
		{
			this.discLibraryRepository = discLibraryRepository ?? throw new ArgumentNullException(nameof(discLibraryRepository));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task Initialize(CancellationToken cancellationToken)
		{
			logger.LogInformation("Loading library data ...");

			var sw = Stopwatch.StartNew();
			var discLibrary = await discLibraryRepository.ReadDiscLibrary(cancellationToken);
			sw.Stop();
			logger.LogInformation($"Library data was loaded in {sw.Elapsed.TotalSeconds:N1} seconds");

			DiscLibraryHolder.DiscLibrary = discLibrary;
		}
	}
}
