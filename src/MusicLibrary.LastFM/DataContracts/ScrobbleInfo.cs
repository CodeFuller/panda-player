using System.Runtime.Serialization;

namespace MusicLibrary.LastFM.DataContracts
{
	[DataContract]
	public class ScrobbleInfo : TrackProcessingInfo
	{
		[DataMember]
		public int Timestamp { get; set; }
	}
}
