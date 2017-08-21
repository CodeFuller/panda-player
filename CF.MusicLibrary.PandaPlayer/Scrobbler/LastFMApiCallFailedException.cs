using System;
using System.Net.Http;
using System.Runtime.Serialization;
using CF.Library.Http.Exceptions;

namespace CF.MusicLibrary.PandaPlayer.Scrobbler
{
	[Serializable]
	public class LastFMApiCallFailedException : ApiCallFailedException
	{
		public LastFMApiCallFailedException()
		{
		}

		internal LastFMApiCallFailedException(HttpResponseMessage response)
			: base(response)
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
