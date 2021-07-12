using System.Runtime.Serialization;

namespace PandaPlayer.LastFM.DataContracts
{
	[DataContract]
	public class UpdateNowPlayingTrackResponse
	{
		[DataMember]
		public TrackProcessingInfo NowPlaying { get; set; }
	}
}
