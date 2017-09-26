using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.PandaPlayer
{
	internal static class FileSizeFormatter
	{
		private const int KiloMultiplier = 1024;
		private const int KbValue = 1 * KiloMultiplier;
		private const int MbValue = 1 * KiloMultiplier * KbValue;
		private const int GbValue = 1 * KiloMultiplier * MbValue;

		public static string GetFormattedFileSize(long fileSize)
		{
			float size = (int)fileSize;

			if (fileSize < KbValue)
			{
				return Current($"{fileSize} B");
			}

			if (fileSize < MbValue)
			{
				return Current($"{size / KbValue:F1} KB");
			}

			if (size < GbValue)
			{
				return Current($"{size / MbValue:F1} MB");
			}

			return Current($"{size / GbValue:F1} GB");
		}
	}
}
