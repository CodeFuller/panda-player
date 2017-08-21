using System.Runtime.Serialization;

namespace CF.MusicLibrary.PandaPlayer.Scrobbler.DataContracts
{
	[DataContract]
	public class ScrobbleTrackResponse
	{
		[DataMember]
		public ScrobblesInfo Scrobbles { get; set; }
	}
}
