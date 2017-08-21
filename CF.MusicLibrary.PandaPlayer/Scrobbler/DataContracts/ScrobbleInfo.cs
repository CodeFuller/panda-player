using System.Runtime.Serialization;

namespace CF.MusicLibrary.PandaPlayer.Scrobbler.DataContracts
{
	[DataContract]
	public class ScrobbleInfo : TrackProcessingInfo
	{
		[DataMember]
		public int Timestamp { get; set; }
	}
}
