using System;
using System.Net.Http;
using System.Runtime.Serialization;

namespace PandaPlayer.LastFM
{
	[Serializable]
	public class LastFMApiCallFailedException : Exception
	{
		public LastFMApiCallFailedException()
		{
		}

		internal LastFMApiCallFailedException(HttpResponseMessage response)
			: this($"LastFM API call has failed. HTTP Code: {response.StatusCode}. ReasonPhrase: {response.ReasonPhrase}")
		{
		}

		public LastFMApiCallFailedException(string message)
			: base(message)
		{
		}

		public LastFMApiCallFailedException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected LastFMApiCallFailedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
