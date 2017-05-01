using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Threading.Tasks;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Objects;
using static System.FormattableString;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.Dal.MediaMonkey
{
	/// <summary>
	/// Implementation of IMusicLibraryRepository for MediaMonkey database stored in Microsoft Access DB.
	/// </summary>
	public class MusicLibraryRepository : IMusicLibraryRepository
	{
		private const string Artists = "Artists";
		private const string Songs = "Songs";
		private const string Genres = "Genres";
		private const string Played = "Played";

		private readonly DbProviderFactory dbProviderFactory;
		private readonly ILibraryBuilder libraryBuilder;
		private readonly string connectionString;
		private readonly string libraryRootDirectory;

		/// <summary>
		/// Constructor.
		/// </summary>
		public MusicLibraryRepository(DbProviderFactory dbProviderFactory, ILibraryBuilder libraryBuilder, string connectionString, string libraryRootDirectory)
		{
			if (dbProviderFactory == null)
			{
				throw new ArgumentNullException(nameof(dbProviderFactory));
			}
			if (libraryBuilder == null)
			{
				throw new ArgumentNullException(nameof(libraryBuilder));
			}

			this.dbProviderFactory = dbProviderFactory;
			this.libraryBuilder = libraryBuilder;
			this.connectionString = connectionString;
			this.libraryRootDirectory = libraryRootDirectory;
		}

		/// <summary>
		/// Implementation of IMusicLibraryRepository.LoadLibrary().
		/// </summary>
		public async Task<DiscLibrary> GetDiscLibraryAsync()
		{
			using (DataSet ds = await LoadData())
			{
				Dictionary<int, Artist> artists = ObjectifyArtists(ds);
				Dictionary<int, Genre> genres = ObjectifyGenres(ds);

				var songsTable = ds.Tables[Songs];
				for (var i = 0; i < songsTable.Rows.Count; ++i)
				{
					DataRow row = songsTable.Rows[i];
					Song song = new Song
					{
						Id = row.Field<int>("ID"),
						Artist = artists[row.GetParentRow(ds.Relations["SongArtist"]).Field<int>("ID")],
						OrderNumber = (short)(row.Field<short>("SongOrder") + 1),
						Year = CheckForNull(row.Field<short>("Year"), (short)-1),
						Title = row.Field<string>("SongTitle"),
						Genre = genres[row.GetParentRow(ds.Relations["SongGenre"]).Field<int>("IDGenre")],
						Duration = TimeSpan.FromMilliseconds(row.Field<int>("SongLength")),
						Rating = CastRating(row.Field<short>("Rating")),
						Uri = GetSongUri(row.Field<string>("SongPath")),
						FileSize = row.Field<int>("FileLength"),
						Bitrate = row.Field<int>("Bitrate"),
						LastPlaybackTime = CheckForNull(row.Field<DateTime>("LastTimePlayed"), d => d.Year <= 1900),
						PlaybacksCount = row.Field<int>("PlayCounter")
					};

					foreach (var playbackRow in row.GetChildRows(ds.Relations["PlaybackSong"]))
					{
						song.Playbacks.Add(new Playback
						{
							PlaybackTime = playbackRow.Field<DateTime>("PlayDate"),
						});
					}

					libraryBuilder.AddSong(song);
				}

				return libraryBuilder.Build();
			}
		}

		public async Task<IEnumerable<Genre>> GetGenresAsync()
		{
			using (DataSet ds = new DataSet { Locale = CultureInfo.InvariantCulture })
			{
				await LoadTableAsync(ds, Genres);
				Dictionary<int, Genre> genres = ObjectifyGenres(ds);
				return genres.Values;
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Object is disposed by the caller.")]
		private async Task<DataSet> LoadData()
		{
			DataSet ds = new DataSet
			{
				Locale = CultureInfo.InvariantCulture
			};

			await LoadTableAsync(ds, Artists);
			await LoadTableAsync(ds, Songs);
			await LoadTableAsync(ds, Genres);
			await LoadTableAsync(ds, Played);

			ds.Relations.Add(new DataRelation("SongArtist",
				ds.Tables[Artists].Columns["ID"],
				ds.Tables[Songs].Columns["IDArtist"]));

			ds.Relations.Add(new DataRelation("SongGenre",
				ds.Tables[Genres].Columns["IDGenre"],
				ds.Tables[Songs].Columns["Genre"]));

			ds.Relations.Add(new DataRelation("PlaybackSong",
				ds.Tables[Songs].Columns["ID"],
				ds.Tables[Played].Columns["IdSong"]));

			return ds;
		}

		private static Dictionary<int, Artist> ObjectifyArtists(DataSet ds)
		{
			DataTable artistsTable = ds.Tables[Artists];
			var artists = new Dictionary<int, Artist>();
			for (var i = 0; i < artistsTable.Rows.Count; ++i)
			{
				var row = artistsTable.Rows[i];
				Artist artist = new Artist
				{
					Id = row.Field<int>("ID"),
					Name = row.Field<string>("Artist"),
				};

				artists.Add(artist.Id, artist);
			}

			return artists;
		}

		private static Dictionary<int, Genre> ObjectifyGenres(DataSet ds)
		{
			DataTable genresTable = ds.Tables[Genres];
			var genres = new Dictionary<int, Genre>();
			for (var i = 0; i < genresTable.Rows.Count; ++i)
			{
				var row = genresTable.Rows[i];
				Genre genre = new Genre
				{
					Id = row.Field<int>("IDGenre"),
					Name = row.Field<string>("GenreName"),
				};

				genres.Add(genre.Id, genre);
			}

			return genres;
		}

		private void LoadTable(DataSet ds, string tableName)
		{
			CreateAdapter(tableName).Fill(ds, tableName);
		}

		private async Task LoadTableAsync(DataSet ds, string tableName)
		{
			await Task.Run(() => { LoadTable(ds, tableName); });
		}

		private DbConnection CreateConnection()
		{
			DbConnection connection = dbProviderFactory.CreateConnection();
			connection.ConnectionString = connectionString;
			return connection;
		}

		private DbDataAdapter CreateAdapter(string tableName)
		{
			var adapter = dbProviderFactory.CreateDataAdapter();
			adapter.SelectCommand = CreateCommand(Invariant($"SELECT * FROM {tableName};"));
			return adapter;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:ReviewSqlQueriesForSecurityVulnerabilities", Justification = "Command text doesn't contain user input.")]
		private DbCommand CreateCommand(string commandText)
		{
			var cmd = CreateConnection().CreateCommand();
			cmd.CommandText = commandText;
			return cmd;
		}

		private static T? CheckForNull<T>(T v, T nullValue = default(T)) where T : struct
		{
			return v.Equals(nullValue) ? (T?) null : v;
		}

		private static T? CheckForNull<T>(T v, Func<T, bool> isNullPredicate) where T : struct
		{
			return isNullPredicate(v) ? (T?)null : v;
		}

		private static Rating? CastRating(short rating)
		{
			return rating == -1 ? null : (Rating?)(rating / 10);
		}

		private Uri GetSongUri(string songPath)
		{
			if (!songPath.StartsWith(libraryRootDirectory, StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException(Current($"Song path '{songPath}' is not within library root directory {libraryRootDirectory}"));
			}

			var relativePath = songPath.Substring(libraryRootDirectory.Length);
			relativePath = relativePath.Replace('\\', '/');

			return new Uri(relativePath, UriKind.Relative);
		}
	}
}
