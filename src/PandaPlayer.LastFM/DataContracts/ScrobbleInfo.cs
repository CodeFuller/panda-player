using System.Runtime.Serialization;

namespace PandaPlayer.LastFM.DataContracts
{
	[DataContract]
	public class ScrobbleInfo : TrackProcessingInfo
	{
		[DataMember]
		public int Timestamp { get; set; }
	}
}
