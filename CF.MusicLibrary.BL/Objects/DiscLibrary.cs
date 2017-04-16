using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace CF.MusicLibrary.BL.Objects
{
	/// <summary>
	/// Data contract for the music library entity.
	/// </summary>
	[DataContract]
	public class DiscLibrary
	{
		/// <summary>
		/// Collection of library discs.
		/// </summary>
		[DataMember]
		public Collection<Disc> Discs { get; } = new Collection<Disc>();
	}
}
