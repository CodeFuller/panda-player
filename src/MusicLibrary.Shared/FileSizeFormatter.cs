namespace MusicLibrary.Shared
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

			return fileSize switch
			{
				< KbValue => $"{fileSize} B",
				< MbValue => $"{size / KbValue:F1} KB",
				< GbValue => $"{size / MbValue:F1} MB",
				_ => $"{size / GbValue:F1} GB"
			};
		}
	}
}
