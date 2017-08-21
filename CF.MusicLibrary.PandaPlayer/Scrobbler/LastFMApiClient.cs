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
using CF.Library.Core.Logging;
using CF.MusicLibrary.PandaPlayer.Scrobbler.DataContracts;
using static System.FormattableString;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.PandaPlayer.Scrobbler
{
	public class LastFMApiClient : ILastFMApiClient
	{
		private static readonly Uri ApiBaseUri = new Uri(@"http://ws.audioscrobbler.com/2.0/");

		private readonly string apiKey;
		private readonly string sharedSecret;
		private string sessionKey;
		private readonly ITokenAuthorizer tokenAuthorizer;
		private readonly IMessageLogger logger;

		public LastFMApiClient(ITokenAuthorizer tokenAuthorizer, IMessageLogger logger, string apiKey, string sharedSecret)
		{
			if (tokenAuthorizer == null)
			{
				throw new ArgumentNullException(nameof(tokenAuthorizer));
			}
			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}

			this.tokenAuthorizer = tokenAuthorizer;
			this.logger = logger;
			this.apiKey = apiKey;
			this.sharedSecret = sharedSecret;
			this.sessionKey = null;
		}

		public LastFMApiClient(ITokenAuthorizer tokenAuthorizer, IMessageLogger logger, string apiKey, string sharedSecret, string sessionKey) :
			this(tokenAuthorizer, logger, apiKey, sharedSecret)
		{
			this.sessionKey = sessionKey;
		}

		public async Task OpenSession()
		{
			if (sessionKey != null)
			{
				return;
			}

			var token = await ObtainRequestToken();
			sessionKey = await ObtainSession(token);
		}

		public async Task UpdateNowPlaying(Track track)
		{
			CheckSession();

			logger.WriteInfo($"Updating current track: [Title: {track.Title}][Artist: {track.Artist}][Album: {track.Album}]");

			var requestParams = new NameValueCollection
			{
				{ "method", "track.updateNowPlaying"},
				{ "artist", track.Artist },
				{ "track", track.Title },
				{ "album", track.Album },
				{ "trackNumber", track.Number.ToString(CultureInfo.InvariantCulture) },
				{ "duration", track.Duration.TotalSeconds.ToString(CultureInfo.InvariantCulture) },
			};

			UpdateNowPlayingTrackResponse response = await PerformPostRequest<UpdateNowPlayingTrackResponse>(requestParams);
			LogCorrections(track, response.NowPlaying);
		}

		public async Task Scrobble(TrackScrobble trackScrobble)
		{
			CheckSession();

			logger.WriteInfo($"Scrobbling track: [Title: {trackScrobble.Track.Title}][Artist: {trackScrobble.Track.Artist}][Album: {trackScrobble.Track.Album}]");

			var requestParams = new NameValueCollection
			{
				{ "method", "track.scrobble" },
				{ "artist", trackScrobble.Track.Artist },
				{ "track", trackScrobble.Track.Title },
				{ "timestamp", ((Int32)trackScrobble.Timestamp.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString(CultureInfo.InvariantCulture) },
				{ "album", trackScrobble.Track.Album },
				{ "choosenByUser", trackScrobble.ChosenByUser ? "1" : "0" },
				{ "trackNumber", trackScrobble.Track.Number.ToString(CultureInfo.InvariantCulture) },
				{ "duration", trackScrobble.Track.Duration.TotalSeconds.ToString(CultureInfo.InvariantCulture) },
			};

			ScrobbleTrackResponse response = await PerformPostRequest<ScrobbleTrackResponse>(requestParams);
			if (response.Scrobbles.Statistics.Ignored > 0)
			{
				logger.WriteWarning($"{response.Scrobbles.Statistics.Ignored} tracks ignored");
			}
			LogCorrections(trackScrobble.Track, response.Scrobbles.Scrobble);
		}

		private void LogCorrections(Track track, TrackProcessingInfo trackInfo)
		{
			if (trackInfo.Artist.Corrected)
			{
				logger.WriteWarning(Current($"Corrected artist: '{track.Artist}' -> '{trackInfo.Artist.Text}'"));
			}
			if (trackInfo.Album.Corrected)
			{
				logger.WriteWarning(Current($"Corrected album: '{track.Album}' -> '{trackInfo.Album.Text}'"));
			}
			if (trackInfo.Track.Corrected)
			{
				logger.WriteWarning(Current($"Corrected track: '{track.Title}' -> '{trackInfo.Track.Text}'"));
			}
			if (!String.IsNullOrEmpty(trackInfo.IgnoredMessage.Text))
			{
				logger.WriteWarning(Current($"Ignored track message: '{trackInfo.IgnoredMessage.Text}'"));
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
					{ "method", "auth.getToken"},
				};

			GetTokenResponse response = await PerformGetRequest<GetTokenResponse>(requestParams);
			var token = response.Token;
			return new UnauthorizedToken(token, $@"http://www.last.fm/api/auth/?api_key={apiKey}&token={token}");
		}

		private async Task<string> ObtainSession(string token)
		{
			var requestParams = new NameValueCollection
				{
					{ "method", "auth.getSession"},
					{ "token", token },
				};

			GetSessionResponse response = await PerformGetRequest<GetSessionResponse>(requestParams);
			return response.Session.Key;
		}

		private void CheckSession()
		{
			if (sessionKey == null)
			{
				throw new InvalidOperationException("Last.fm API Session is not opened");
			}
		}

		private async Task<TData> PerformGetRequest<TData>(NameValueCollection requestParams) where TData : class
		{
			using (var request = CreateGetHttpRequest(new Uri("?" + BuildApiMethodQueryString(requestParams), UriKind.Relative)))
			{
				return await PerformHttpRequest<TData>(request);
			}
		}

		private async Task<TData> PerformPostRequest<TData>(NameValueCollection requestParams) where TData : class
		{
			using (var request = CreatePostHttpRequest(requestParams))
			{
				return await PerformHttpRequest<TData>(request);
			}
		}

		private async Task<TData> PerformHttpRequest<TData>(HttpRequestMessage request) where TData : class
		{
			using (var client = GetHttpClient())
			{
				var response = await client.SendAsync(request);
				return await ParseResponseAsync<TData>(response);
			}
		}

		private static async Task<TData> ParseResponseAsync<TData>(HttpResponseMessage response) where TData : class
		{
			try
			{
				var responseData = await response.Content.ReadAsAsync<TData>();
				if (response.IsSuccessStatusCode)
				{
					return responseData;
				}
			}
			catch (Exception)
			{
			}

			throw new LastFMApiCallFailedException(response);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Object is disposed by the caller.")]
		private static HttpClient GetHttpClient()
		{
			var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }, true)
			{
				BaseAddress = ApiBaseUri,
			};
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			return client;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Object is disposed by the caller.")]
		private static HttpRequestMessage CreateGetHttpRequest(Uri relativeUri)
		{
			return new HttpRequestMessage
			{
				RequestUri = relativeUri,
				Method = HttpMethod.Get,
			};
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Object is disposed by the caller.")]
		private HttpRequestMessage CreatePostHttpRequest(NameValueCollection requestParams)
		{
			return new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				Content = new FormUrlEncodedContent(GetNameValueCollectionData(BuildApiMethodRequestParams(requestParams))),
			};
		}

		private NameValueCollection BuildApiMethodRequestParams(NameValueCollection requestParams)
		{
			var requestParamsWithSignature = new NameValueCollection(requestParams);
			requestParamsWithSignature.Add("api_key", apiKey);
			if (sessionKey != null)
			{
				requestParamsWithSignature.Add("sk", sessionKey);
			}
			requestParamsWithSignature.Add("api_sig", CalcCallSign(requestParamsWithSignature));

			//	'format' is not a part of method signature.
			//	That's why it's not in signed part.
			requestParamsWithSignature.Add("format", "json");

			return requestParamsWithSignature;
		}

		private string BuildApiMethodQueryString(NameValueCollection requestParams)
		{
			return BuildQueryString(BuildApiMethodRequestParams(requestParams));
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
			var paramsValues = from key in requestParams.AllKeys orderby key
							   from value in requestParams.GetValues(key)
							   select Invariant($"{key}{value}");

			var signedData = String.Join(String.Empty, paramsValues);
			signedData += sharedSecret;

			return CalcMD5(signedData);
		}

		private static IEnumerable<KeyValuePair<string, string>> GetNameValueCollectionData(NameValueCollection collection)
		{
			return from key in collection.AllKeys
				from value in collection.GetValues(key)
				select new KeyValuePair<string, string>(key, value);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Lower case is an external requirement.")]
		private static string CalcMD5(string data)
		{
			using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
			{
				byte[] dataToHash = new UTF8Encoding().GetBytes(data);
				byte[] hashBytes = md5.ComputeHash(dataToHash);
				return BitConverter.ToString(hashBytes)
					.Replace("-", string.Empty)
					.ToLower(CultureInfo.InvariantCulture);
			}
		}
	}
}
