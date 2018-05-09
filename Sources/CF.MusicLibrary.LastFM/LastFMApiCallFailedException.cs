using System;
using System.Net.Http;
using System.Runtime.Serialization;
using CF.Library.Core.Exceptions;

namespace CF.MusicLibrary.LastFM
{
	[Serializable]
	public class LastFMApiCallFailedException : ApiCallFailedException
	{
		public LastFMApiCallFailedException()
		{
		}

		internal LastFMApiCallFailedException(HttpResponseMessage response)
			: base(response.ReasonPhrase ?? String.Empty)
		{
			RequestUri = response.RequestMessage.RequestUri;
			HttpStatusCode = (int)(response.StatusCode);
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
