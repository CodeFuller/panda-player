using System.Runtime.Serialization;

namespace CF.MusicLibrary.BL.Objects
{
	/// <summary>
	/// Data contract for the Artist entity.
	/// </summary>
	[DataContract]
	public class Artist
	{
		/// <summary>
		/// Artist id.
		/// </summary>
		[DataMember]
		public int Id { get; set; }

		/// <summary>
		/// Artist name.
		/// </summary>
		[DataMember]
		public string Name { get; set; }
	}
}
