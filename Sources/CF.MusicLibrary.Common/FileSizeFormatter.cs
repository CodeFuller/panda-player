using CF.Library.Core.Extensions;

namespace CF.MusicLibrary.Common
{
	public static class FileSizeFormatter
	{
		private const int KiloMultiplier = 1024;
		private const int KbValue = 1 * KiloMultiplier;
		private const int MbValue = 1 * KiloMultiplier * KbValue;
		private const int GbValue = 1 * KiloMultiplier * MbValue;

		public static string GetFormattedFileSize(long fileSize)
		{
			float size = fileSize;

			if (fileSize < KbValue)
			{
				return FormattableStringExtensions.Current($"{fileSize} B");
			}

			if (fileSize < MbValue)
			{
				return FormattableStringExtensions.Current($"{size / KbValue:F1} KB");
			}

			if (size < GbValue)
			{
				return FormattableStringExtensions.Current($"{size / MbValue:F1} MB");
			}

			return FormattableStringExtensions.Current($"{size / GbValue:F1} GB");
		}
	}
}
