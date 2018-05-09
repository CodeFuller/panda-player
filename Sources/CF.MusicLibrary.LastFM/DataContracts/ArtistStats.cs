using System.Runtime.Serialization;

namespace CF.MusicLibrary.LastFM.DataContracts
{
	[DataContract]
	public class ArtistStats
	{
		[DataMember]
		public int Listeners { get; set; }

		[DataMember]
		public int PlayCount { get; set; }

		[DataMember]
		public int UserPlayCount { get; set; }
	}
}
