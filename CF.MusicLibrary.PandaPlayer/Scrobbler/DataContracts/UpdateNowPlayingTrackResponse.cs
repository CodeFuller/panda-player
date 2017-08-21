using System.Runtime.Serialization;

namespace CF.MusicLibrary.PandaPlayer.Scrobbler.DataContracts
{
	[DataContract]
	public class UpdateNowPlayingTrackResponse
	{
		[DataMember]
		public TrackProcessingInfo NowPlaying { get; set; }
	}
}
