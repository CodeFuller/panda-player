using System.Runtime.Serialization;

namespace CF.MusicLibrary.LastFM.DataContracts
{
	[DataContract]
	public class ScrobbleInfo : TrackProcessingInfo
	{
		[DataMember]
		public int Timestamp { get; set; }
	}
}
