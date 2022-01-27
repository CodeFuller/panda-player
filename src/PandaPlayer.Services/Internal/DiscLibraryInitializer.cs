using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Services.Interfaces.Dal;

namespace PandaPlayer.Services.Internal
{
	internal class DiscLibraryInitializer : IApplicationInitializer
	{
		private readonly IFoldersRepository foldersRepository;

		private readonly IDiscsRepository discsRepository;

		private readonly IArtistsRepository artistsRepository;

		private readonly IGenresRepository genresRepository;

		private readonly IAdviseGroupRepository adviseGroupRepository;

		private readonly IAdviseSetRepository adviseSetRepository;

		private readonly ILogger<DiscLibraryInitializer> logger;

		public DiscLibraryInitializer(IFoldersRepository foldersRepository, IDiscsRepository discsRepository,
			IArtistsRepository artistsRepository, IGenresRepository genresRepository, IAdviseGroupRepository adviseGroupRepository,
			IAdviseSetRepository adviseSetRepository, ILogger<DiscLibraryInitializer> logger)
		{
			this.foldersRepository = foldersRepository ?? throw new ArgumentNullException(nameof(foldersRepository));
			this.discsRepository = discsRepository ?? throw new ArgumentNullException(nameof(discsRepository));
			this.artistsRepository = artistsRepository ?? throw new ArgumentNullException(nameof(artistsRepository));
			this.genresRepository = genresRepository ?? throw new ArgumentNullException(nameof(genresRepository));
			this.adviseGroupRepository = adviseGroupRepository ?? throw new ArgumentNullException(nameof(adviseGroupRepository));
			this.adviseSetRepository = adviseSetRepository ?? throw new ArgumentNullException(nameof(adviseSetRepository));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task Initialize(CancellationToken cancellationToken)
		{
			logger.LogInformation("Loading library data ...");

			var discs = await discsRepository.GetAllDiscs(cancellationToken);
			var foldersWithoutDiscs = await foldersRepository.GetFoldersWithoutDiscs(cancellationToken);
			var emptyArtists = await artistsRepository.GetEmptyArtists(cancellationToken);
			var emptyGenres = await genresRepository.GetEmptyGenres(cancellationToken);
			var emptyAdviseGroups = await adviseGroupRepository.GetEmptyAdviseGroups(cancellationToken);
			var emptyAdviseSets = await adviseSetRepository.GetEmptyAdviseSets(cancellationToken);

			var discLibrary = new DiscLibrary(discs, foldersWithoutDiscs, emptyArtists, emptyGenres, emptyAdviseGroups, emptyAdviseSets);
			DiscLibraryHolder.DiscLibrary = discLibrary;

			logger.LogInformation("Library data was loaded successfully");
		}
	}
}
