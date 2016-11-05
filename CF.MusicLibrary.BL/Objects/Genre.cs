using System.Runtime.Serialization;

namespace CF.MusicLibrary.BL.Objects
{
	/// <summary>
	/// Data contract for the musical genre entity.
	/// </summary>
	[DataContract]
	public class Genre
	{
		/// <summary>
		/// Genre id.
		/// </summary>
		[DataMember]
		public int Id { get; set; }

		/// <summary>
		/// Genre name.
		/// </summary>
		[DataMember]
		public string Name { get; set; }
	}
}
