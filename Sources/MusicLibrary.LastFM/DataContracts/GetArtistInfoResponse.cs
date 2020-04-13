using System.Runtime.Serialization;

namespace MusicLibrary.LastFM.DataContracts
{
	[DataContract]
	public class GetArtistInfoResponse
	{
		[DataMember]
		public ArtistInfo Artist { get; set; }
	}
}
