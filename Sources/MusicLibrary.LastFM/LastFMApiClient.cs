using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MusicLibrary.LastFM.DataContracts;
using MusicLibrary.LastFM.Objects;
using Newtonsoft.Json;
using static System.FormattableString;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.LastFM
{
	public class LastFMApiClient : ILastFMApiClient
	{
		private static readonly Uri ApiBaseUri = new Uri(@"http://ws.audioscrobbler.com/2.0/");

		private readonly ITokenAuthorizer tokenAuthorizer;
		private readonly ILogger<LastFMApiClient> logger;
		private readonly LastFmClientSettings settings;

		public LastFMApiClient(ITokenAuthorizer tokenAuthorizer, ILogger<LastFMApiClient> logger, IOptions<LastFmClientSettings> options)
		{
			this.tokenAuthorizer = tokenAuthorizer ?? throw new ArgumentNullException(nameof(tokenAuthorizer));
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task OpenSession()
		{
			if (!String.IsNullOrEmpty(settings.SessionKey))
			{
				return;
			}

			var token = await ObtainRequestToken();
			settings.SessionKey = await ObtainSession(token);
		}

		public async Task UpdateNowPlaying(Track track)
		{
			if (!CheckSession())
			{
				return;
			}

			ValidateScrobbledTrack(track);

			logger.LogInformation($"Updating current track: [Title: {track.Title}][Artist: {track.Artist}][Album: {track.Album.Title}]");

			var requestParams = new NameValueCollection
			{
				{ "method", "track.updateNowPlaying" },
				{ "artist", track.Artist },
				{ "track", track.Title },
				{ "duration", track.Duration.TotalSeconds.ToString(CultureInfo.InvariantCulture) },
			};

			if (!String.IsNullOrEmpty(track.Album.Title))
			{
				requestParams.Add("album", track.Album.Title);
			}

			if (track.Number.HasValue)
			{
				requestParams.Add("trackNumber", track.Number.Value.ToString(CultureInfo.InvariantCulture));
			}

			UpdateNowPlayingTrackResponse response = await PerformPostRequest<UpdateNowPlayingTrackResponse>(requestParams, true);
			LogCorrections(track, response.NowPlaying);
		}

		public async Task Scrobble(TrackScrobble trackScrobble)
		{
			if (!CheckSession())
			{
				return;
			}

			ValidateScrobbledTrack(trackScrobble.Track);

			logger.LogInformation($"Scrobbling track: [Title: {trackScrobble.Track.Title}][Artist: {trackScrobble.Track.Artist}][Album: {trackScrobble.Track.Album.Title}]");

			var requestParams = new NameValueCollection
			{
				{ "method", "track.scrobble" },
				{ "artist", trackScrobble.Track.Artist },
				{ "track", trackScrobble.Track.Title },
				{ "timestamp", ((Int32)trackScrobble.PlayStartTimestamp.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString(CultureInfo.InvariantCulture) },
				{ "choosenByUser", trackScrobble.ChosenByUser ? "1" : "0" },
				{ "duration", trackScrobble.Track.Duration.TotalSeconds.ToString(CultureInfo.InvariantCulture) },
			};
			if (!String.IsNullOrEmpty(trackScrobble.Track.Album.Title))
			{
				requestParams.Add("album", trackScrobble.Track.Album.Title);
			}

			if (trackScrobble.Track.Number.HasValue)
			{
				requestParams.Add("trackNumber", trackScrobble.Track.Number.Value.ToString(CultureInfo.InvariantCulture));
			}

			ScrobbleTrackResponse response = await PerformPostRequest<ScrobbleTrackResponse>(requestParams, true);
			if (response.Scrobbles.Statistics.Ignored > 0)
			{
				logger.LogWarning($"{response.Scrobbles.Statistics.Ignored} tracks ignored");
			}

			LogCorrections(trackScrobble.Track, response.Scrobbles.Scrobble);
		}

		public async Task<GetArtistInfoResponse> GetArtistInfo(string artistName, string userName)
		{
			var requestParams = new NameValueCollection
			{
				{ "method", "artist.getInfo" },
				{ "artist", artistName },
				{ "autocorrect", "1" },
				{ "userName", userName },
			};

			return await PerformGetRequest<GetArtistInfoResponse>(requestParams, false);
		}

		public async Task<GetAlbumInfoResponse> GetAlbumInfo(Album album, string userName)
		{
			var requestParams = new NameValueCollection
			{
				{ "method", "album.getInfo" },
				{ "artist", album.Artist },
				{ "album", album.Title },
				{ "autocorrect", "1" },
				{ "userName", userName },
			};

			return await PerformGetRequest<GetAlbumInfoResponse>(requestParams, false);
		}

		public async Task<GetTrackInfoResponse> GetTrackInfo(Track track, string userName)
		{
			var requestParams = new NameValueCollection
			{
				{ "method", "track.getInfo" },
				{ "track", track.Title },
				{ "artist", track.Artist },
				{ "autocorrect", "1" },
				{ "userName", userName },
			};

			return await PerformGetRequest<GetTrackInfoResponse>(requestParams, false);
		}

		private static void ValidateScrobbledTrack(Track track)
		{
			if (String.IsNullOrEmpty(track.Artist))
			{
				throw new InvalidOperationException("Could not scrobble track without an Artist");
			}

			if (String.IsNullOrEmpty(track.Title))
			{
				throw new InvalidOperationException("Could not scrobble track without a Title");
			}
		}

		private void LogCorrections(Track track, TrackProcessingInfo trackInfo)
		{
			if (trackInfo.Artist.Corrected)
			{
				logger.LogWarning(Current($"Corrected artist: '{track.Artist}' -> '{trackInfo.Artist.Text}'"));
			}

			if (trackInfo.Album.Corrected)
			{
				logger.LogWarning(Current($"Corrected album: '{track.Album}' -> '{trackInfo.Album.Text}'"));
			}

			if (trackInfo.Track.Corrected)
			{
				logger.LogWarning(Current($"Corrected track: '{track.Title}' -> '{trackInfo.Track.Text}'"));
			}

			if (!String.IsNullOrEmpty(trackInfo.IgnoredMessage.Text))
			{
				logger.LogWarning(Current($"Ignored track message: '{trackInfo.IgnoredMessage.Text}'"));
			}
		}

		private async Task<string> ObtainRequestToken()
		{
			UnauthorizedToken unauthorizedToken = await GetAuthenticationToken();
			return await tokenAuthorizer.AuthorizeToken(unauthorizedToken);
		}

		private async Task<UnauthorizedToken> GetAuthenticationToken()
		{
			var requestParams = new NameValueCollection
				{
					{ "method", "auth.getToken" },
				};

			GetTokenResponse response = await PerformGetRequest<GetTokenResponse>(requestParams, false);
			var token = response.Token;
			return new UnauthorizedToken(token, $@"http://www.last.fm/api/auth/?api_key={settings.ApiKey}&token={token}");
		}

		private async Task<string> ObtainSession(string token)
		{
			var requestParams = new NameValueCollection
				{
					{ "method", "auth.getSession" },
					{ "token", token },
				};

			GetSessionResponse response = await PerformGetRequest<GetSessionResponse>(requestParams, false);
			return response.Session.Key;
		}

		private bool CheckSession()
		{
			if (String.IsNullOrEmpty(settings.SessionKey))
			{
				logger.LogWarning("Session key is not set for Last.FM client. No tracks will be scrobbled");
				return false;
			}

			return true;
		}

		private async Task<TData> PerformGetRequest<TData>(NameValueCollection requestParams, bool requiresAuth)
			where TData : class
		{
			using (var request = CreateGetHttpRequest(new Uri("?" + BuildApiMethodQueryString(requestParams, requiresAuth), UriKind.Relative)))
			{
				return await PerformHttpRequest<TData>(request);
			}
		}

		private async Task<TData> PerformPostRequest<TData>(NameValueCollection requestParams, bool requiresAuthentication)
			where TData : class
		{
			using (var request = CreatePostHttpRequest(requestParams, requiresAuthentication))
			{
				return await PerformHttpRequest<TData>(request);
			}
		}

		private static async Task<TData> PerformHttpRequest<TData>(HttpRequestMessage request)
			where TData : class
		{
			using var clientHandler = new HttpClientHandler
			{
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
			};

			using var client = new HttpClient(clientHandler, true)
			{
				BaseAddress = ApiBaseUri,
			};

			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			var response = await client.SendAsync(request);
			return await ParseResponseAsync<TData>(response);
		}

		private static async Task<TData> ParseResponseAsync<TData>(HttpResponseMessage response)
			where TData : class
		{
			try
			{
				var responseContent = await response.Content.ReadAsStringAsync();
				var responseData = JsonConvert.DeserializeObject<TData>(responseContent);
				if (response.IsSuccessStatusCode)
				{
					return responseData;
				}
			}
#pragma warning disable CA1031 // Do not catch general exception types
			catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
			{
			}

			throw new LastFMApiCallFailedException(response);
		}

		private static HttpRequestMessage CreateGetHttpRequest(Uri relativeUri)
		{
			return new HttpRequestMessage
			{
				RequestUri = relativeUri,
				Method = HttpMethod.Get,
			};
		}

		private HttpRequestMessage CreatePostHttpRequest(NameValueCollection requestParams, bool requiresAuthentication)
		{
			return new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				Content = new FormUrlEncodedContent(GetNameValueCollectionData(BuildApiMethodRequestParams(requestParams, requiresAuthentication))),
			};
		}

		private NameValueCollection BuildApiMethodRequestParams(NameValueCollection requestParams, bool requiresAuthentication)
		{
			var requestParamsWithSignature = new NameValueCollection(requestParams);
			requestParamsWithSignature.Add("api_key", settings.ApiKey);
			if (requiresAuthentication)
			{
				if (String.IsNullOrEmpty(settings.SessionKey))
				{
					throw new InvalidOperationException("Session key for Last.FM client is not set");
				}

				requestParamsWithSignature.Add("sk", settings.SessionKey);
			}

			requestParamsWithSignature.Add("api_sig", CalcCallSign(requestParamsWithSignature));

			// 'format' is not a part of method signature.
			// That's why it's not in signed part.
			requestParamsWithSignature.Add("format", "json");

			return requestParamsWithSignature;
		}

		private string BuildApiMethodQueryString(NameValueCollection requestParams, bool requiresAuthentication)
		{
			return BuildQueryString(BuildApiMethodRequestParams(requestParams, requiresAuthentication));
		}

		private static string BuildQueryString(NameValueCollection requestParams)
		{
			var paramsValues = from key in requestParams.AllKeys
							   from value in requestParams.GetValues(key)
							   select Invariant($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}");

			return String.Join("&", paramsValues);
		}

		private string CalcCallSign(NameValueCollection requestParams)
		{
			var paramsValues = from key in requestParams.AllKeys
							   orderby key
							   from value in requestParams.GetValues(key)
							   select Invariant($"{key}{value}");

			var signedData = String.Join(String.Empty, paramsValues);
			signedData += settings.SharedSecret;

			return CalcMD5(signedData);
		}

		private static IEnumerable<KeyValuePair<string, string>> GetNameValueCollectionData(NameValueCollection collection)
		{
			return from key in collection.AllKeys
				from value in collection.GetValues(key)
				select new KeyValuePair<string, string>(key, value);
		}

		private static string CalcMD5(string data)
		{
#pragma warning disable CA5351 // Do Not Use Broken Cryptographic Algorithms - We are just clients of 3rd party API and use algorithm required by the server
			using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
#pragma warning restore CA5351 // Do Not Use Broken Cryptographic Algorithms
			{
				byte[] dataToHash = new UTF8Encoding().GetBytes(data);
				byte[] hashBytes = md5.ComputeHash(dataToHash);

#pragma warning disable CA1308 // Normalize strings to uppercase - Using of lower case is an external requirement
				return BitConverter.ToString(hashBytes)
					.Replace("-", string.Empty)
					.ToLower(CultureInfo.InvariantCulture);
#pragma warning restore CA1308 // Normalize strings to uppercase
			}
		}
	}
}
