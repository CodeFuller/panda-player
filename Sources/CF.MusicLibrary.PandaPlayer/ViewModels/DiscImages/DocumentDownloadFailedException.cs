using System;
using System.Runtime.Serialization;
using CF.Library.Core.Exceptions;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.DiscImages
{
	[Serializable]
	public class DocumentDownloadFailedException : BasicException
	{
		public DocumentDownloadFailedException()
		{
		}

		public DocumentDownloadFailedException(string message)
			: base(message)
		{
		}

		public DocumentDownloadFailedException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected DocumentDownloadFailedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
