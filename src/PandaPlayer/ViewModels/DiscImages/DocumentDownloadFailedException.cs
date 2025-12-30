using System;

namespace PandaPlayer.ViewModels.DiscImages
{
	[Serializable]
	public class DocumentDownloadFailedException : Exception
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
	}
}
