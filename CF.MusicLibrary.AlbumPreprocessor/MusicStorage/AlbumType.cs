namespace CF.MusicLibrary.AlbumPreprocessor.MusicStorage
{
	public enum AlbumType
	{
		/// <summary>
		/// Invalid value.
		/// </summary>
		Undefined,

		/// <summary>
		/// Regular artist album, e.g. "Epica\2007 - The Divine Conspiracy"
		/// </summary>
		ArtistAlbum,

		/// <summary>
		/// Compilation album with artist info in song filename, e.g. "Soundtracks\The Matrix\03 - Ministry - Bad Blood.mp3"
		/// </summary>
		CompilationAlbumWithArtistInfo,

		/// <summary>
		/// Compilation album without artist info in song filename, e.g. "Soundtracks\Gladiator\01 - Progeny.mp3"
		/// </summary>
		CompilationAlbumWithoutArtistInfo,
	}
}
