using System.Runtime.Serialization;

namespace CF.MusicLibrary.LastFM.DataContracts
{
	[DataContract]
	public class ArtistInfo
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public ArtistStats Stats { get; set; }
	}
}
