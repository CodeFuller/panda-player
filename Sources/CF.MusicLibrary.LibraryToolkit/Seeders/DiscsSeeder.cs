using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CF.MusicLibrary.Core;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.LibraryToolkit.Extensions;
using CF.MusicLibrary.LibraryToolkit.Interfaces;
using CF.MusicLibrary.LibraryToolkit.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MusicLibraryApi.Client.Contracts.Discs;
using MusicLibraryApi.Client.Interfaces;

namespace CF.MusicLibrary.LibraryToolkit.Seeders
{
	public class DiscsSeeder : IDiscsSeeder
	{
		private readonly IDiscsMutation discsMutation;

		private readonly ILogger<DiscsSeeder> logger;

		private readonly DiscSeederSettings settings;

		public DiscsSeeder(IDiscsMutation discsMutation, ILogger<DiscsSeeder> logger, IOptions<DiscSeederSettings> options)
		{
			this.discsMutation = discsMutation ?? throw new ArgumentNullException(nameof(discsMutation));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		public async Task<IReadOnlyDictionary<int, int>> SeedDiscs(DiscLibrary discLibrary, IReadOnlyDictionary<Uri, int> folders, CancellationToken cancellationToken)
		{
			logger.LogInformation("Seeding discs ...");

			// Disc Uri -> (Album Id, Album Order)
			var discAlbumsInfo = BuildDiscsAlbumsInfo(discLibrary);

			var discs = new Dictionary<int, int>();

			foreach (var disc in discLibrary.AllDiscs.OrderBy(d => d.Uri.ToString()))
			{
				var folderId = GetDiscFolderId(disc, folders);

				var treeTitle = disc.Uri.GetLastPart();

				var deleteDate = disc.IsDeleted ? disc.AllSongs.Select(s => s.DeleteDate).Max() : null;
				var deleteComment = deleteDate != null ? String.Empty : null;

				var albumInfo = discAlbumsInfo[disc.Uri];
				var discData = new InputDiscData(folderId: folderId, year: disc.Year, title: disc.Title, treeTitle: treeTitle, albumTitle: disc.AlbumTitle ?? disc.Title,
					albumId: albumInfo.Item1, albumOrder: albumInfo.Item2, deleteDate: deleteDate, deleteComment: deleteComment);

				var discId = await discsMutation.CreateDisc(discData, cancellationToken);
				discs.Add(disc.Id, discId);
			}

			logger.LogInformation("Seeded {DiscsNumber} discs", discs.Count);

			return discs;
		}

		private IDictionary<Uri, (string, int?)> BuildDiscsAlbumsInfo(DiscLibrary discLibrary)
		{
			var explicitAlbumsInfo = LoadExplicitAlbumsInfo();
			var discAlbumsInfo = new Dictionary<Uri, (string, int?)>();

			foreach (var albumDiscs in discLibrary.AllDiscs.GroupBy(d => $"{GetDiscFolderUri(d)}|{d.AlbumTitle}").OrderBy(g => g.Key))
			{
				if (albumDiscs.Count() == 1)
				{
					var disc = albumDiscs.Single();

					if (!explicitAlbumsInfo.TryGetValue(disc.Uri, out var albumInfo))
					{
						// Sanity check
						var discNumber = ParseDiscAlbumOrder(disc);
						if (discNumber.HasValue)
						{
							throw new InvalidOperationException($"Disc with album order has no siblings: '{disc.Title}'");
						}

						albumInfo = (null, null);
					}

					discAlbumsInfo.Add(disc.Uri, albumInfo);
				}
				else
				{
					var albumId = Guid.NewGuid().ToString("B".ToUpperInvariant(), CultureInfo.InvariantCulture);
					foreach (var disc in albumDiscs)
					{
						if (!explicitAlbumsInfo.TryGetValue(disc.Uri, out var albumInfo))
						{
							albumInfo = (albumId, GetDiscAlbumOrder(disc));
						}

						discAlbumsInfo.Add(disc.Uri, albumInfo);
					}
				}
			}

			return discAlbumsInfo;
		}

		private IDictionary<Uri, (string, int?)> LoadExplicitAlbumsInfo()
		{
			var albumsInfo = new Dictionary<Uri, (string, int?)>();

			if (String.IsNullOrEmpty(settings.ExplicitAlbumsInfoFile))
			{
				return albumsInfo;
			}

			foreach (var line in File.ReadLines(settings.ExplicitAlbumsInfoFile))
			{
				if (line.Length == 0 || line.StartsWith(";", StringComparison.Ordinal))
				{
					continue;
				}

				var values = line.Split(',');
				if (values.Length != 3)
				{
					throw new InvalidOperationException($"File '{settings.ExplicitAlbumsInfoFile}' contains invalid line: '{line}'");
				}

				var albumId = values[1] == "null" ? null : values[1];

				var albumOrderValue = values[2];
				var albumOrder = albumOrderValue == "null" ? (int?)null : Int32.Parse(albumOrderValue, NumberStyles.None, CultureInfo.InvariantCulture);

				albumsInfo.Add(new Uri(values[0], UriKind.Relative), (albumId, albumOrder));
			}

			return albumsInfo;
		}

		private static int GetDiscFolderId(Disc disc, IReadOnlyDictionary<Uri, int> folders)
		{
			var folderUri = GetDiscFolderUri(disc);
			if (folders.TryGetValue(folderUri, out var folderId))
			{
				return folderId;
			}

			if (!disc.IsDeleted)
			{
				throw new InvalidOperationException($"Failed to get folder id for Uri '{folderUri}'");
			}

			// We return root folder for deleted disc with missing parent folder.
			if (folders.TryGetValue(new Uri("/", UriKind.Relative), out folderId))
			{
				return folderId;
			}

			throw new InvalidOperationException("Root folder id is unknown");
		}

		private static Uri GetDiscFolderUri(Disc disc)
		{
			var parts = new ItemUriParts(disc.Uri).ToList();
			return ItemUriParts.Join(parts.Take(parts.Count - 1));
		}

		private static int? ParseDiscAlbumOrder(Disc disc)
		{
			var re = new Regex(@"^.+\(CD (\d+)\)$");
			var match = re.Match(disc.Title);
			if (match.Success)
			{
				return Int32.Parse(match.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture);
			}

			return null;
		}

		private static int GetDiscAlbumOrder(Disc disc)
		{
			var albumOrder = ParseDiscAlbumOrder(disc);
			if (albumOrder.HasValue)
			{
				return albumOrder.Value;
			}

			throw new InvalidOperationException($"Can not determine album order for disc title {disc.Title}");
		}
	}
}
