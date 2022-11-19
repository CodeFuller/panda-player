using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PandaPlayer.LastFM.DataContracts;
using PandaPlayer.LastFM.Interfaces;
using PandaPlayer.LastFM.Objects;

namespace PandaPlayer.LastFM.Internal
{
	internal class LastFMApiClient : ILastFMApiClient
	{
		private readonly ILogger<LastFMApiClient> logger;
		private readonly LastFmClientSettings settings;

		public LastFMApiClient(ILogger<LastFMApiClient> logger, IOptions<LastFmClientSettings> options)
		{
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task UpdateNowPlaying(Track track, CancellationToken cancellationToken)
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

			var response = await PerformPostRequest<UpdateNowPlayingTrackResponse>(requestParams, true, cancellationToken);
			LogCorrections(track, response.NowPlaying);
		}

		public async Task Scrobble(TrackScrobble trackScrobble, CancellationToken cancellationToken)
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
				{ "timestamp", trackScrobble.PlayStartTimestamp.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture) },
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

			var response = await PerformPostRequest<ScrobbleTrackResponse>(requestParams, true, cancellationToken);
			if (response.Scrobbles.Statistics.Ignored > 0)
			{
				logger.LogWarning($"{response.Scrobbles.Statistics.Ignored} tracks ignored");
			}

			LogCorrections(trackScrobble.Track, response.Scrobbles.Scrobble);
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
				logger.LogWarning($"Corrected artist: '{track.Artist}' -> '{trackInfo.Artist.Text}'");
			}

			if (trackInfo.Album.Corrected)
			{
				logger.LogWarning($"Corrected album: '{track.Album}' -> '{trackInfo.Album.Text}'");
			}

			if (trackInfo.Track.Corrected)
			{
				logger.LogWarning($"Corrected track: '{track.Title}' -> '{trackInfo.Track.Text}'");
			}

			if (!String.IsNullOrEmpty(trackInfo.IgnoredMessage.Text))
			{
				logger.LogWarning($"Ignored track message: '{trackInfo.IgnoredMessage.Text}'");
			}
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

		private async Task<TData> PerformPostRequest<TData>(NameValueCollection requestParams, bool requiresAuthentication, CancellationToken cancellationToken)
			where TData : class
		{
			using var request = CreatePostHttpRequest(requestParams, requiresAuthentication);
			return await PerformHttpRequest<TData>(request, cancellationToken);
		}

		private async Task<TData> PerformHttpRequest<TData>(HttpRequestMessage request, CancellationToken cancellationToken)
			where TData : class
		{
			using var clientHandler = new HttpClientHandler
			{
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
				CheckCertificateRevocationList = true,
			};

			using var client = new HttpClient(clientHandler, true)
			{
				BaseAddress = settings.ApiBaseUri,
			};

			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			var response = await client.SendAsync(request, cancellationToken);
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

		private string CalcCallSign(NameValueCollection requestParams)
		{
			var paramsValues = from key in requestParams.AllKeys
							   orderby key
							   from value in requestParams.GetValues(key)
							   select $"{key}{value}";

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
#pragma warning disable CA5351 // Do Not Use Broken Cryptographic Algorithms - We are clients of 3rd party API and use algorithm required by the server.
			using var md5 = MD5.Create();
#pragma warning restore CA5351 // Do Not Use Broken Cryptographic Algorithms

			var dataToHash = new UTF8Encoding().GetBytes(data);
			var hashBytes = md5.ComputeHash(dataToHash);

#pragma warning disable CA1308 // Normalize strings to uppercase - Using of lower case is an external requirement.
			return BitConverter.ToString(hashBytes)
				.Replace("-", string.Empty, StringComparison.Ordinal)
				.ToLower(CultureInfo.InvariantCulture);
#pragma warning restore CA1308 // Normalize strings to uppercase
		}
	}
}
