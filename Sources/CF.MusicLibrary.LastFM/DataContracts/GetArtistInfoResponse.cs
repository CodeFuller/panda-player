using System.Runtime.Serialization;

namespace CF.MusicLibrary.LastFM.DataContracts
{
	[DataContract]
	public class GetArtistInfoResponse
	{
		[DataMember]
		public ArtistInfo Artist { get; set; }
	}
}
