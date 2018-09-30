using System.Runtime.Serialization;

namespace CF.MusicLibrary.LastFM.DataContracts
{
	[DataContract]
	public class UpdateNowPlayingTrackResponse
	{
		[DataMember]
		public TrackProcessingInfo NowPlaying { get; set; }
	}
}
