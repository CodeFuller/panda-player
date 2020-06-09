namespace MusicLibrary.DiscAdder.MusicStorage
{
	public enum DsicType
	{
		/// <summary>
		/// Invalid value.
		/// </summary>
		Undefined,

		/// <summary>
		/// Regular artist disc, e.g. "Epica\2007 - The Divine Conspiracy"
		/// </summary>
		ArtistDisc,

		/// <summary>
		/// Compilation disc with artist info in song filename, e.g. "Soundtracks\The Matrix\03 - Ministry - Bad Blood.mp3"
		/// </summary>
		CompilationDiscWithArtistInfo,

		/// <summary>
		/// Compilation disc without artist info in song filename, e.g. "Soundtracks\Gladiator\01 - Progeny.mp3"
		/// </summary>
		CompilationDiscWithoutArtistInfo,
	}
}
