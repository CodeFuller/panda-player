using System;

namespace PandaPlayer.Services.Diagnostic
{
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
#pragma warning disable CA2217 // Do not mark enums with FlagsAttribute
	[Flags]
	public enum LibraryCheckFlags
#pragma warning restore CA2217 // Do not mark enums with FlagsAttribute
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
	{
		None = 0,
		CheckDiscsConsistency = 0x01,
		CheckStorageConsistency = 0x02,
		CheckContentConsistency = CheckStorageConsistency | 0x04,
		CheckSongTagsConsistency = CheckStorageConsistency | 0x08,
		All = CheckDiscsConsistency | CheckContentConsistency | CheckSongTagsConsistency,
	}
}
