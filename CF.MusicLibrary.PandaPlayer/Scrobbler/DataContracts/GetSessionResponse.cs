using System.Runtime.Serialization;

namespace CF.MusicLibrary.PandaPlayer.Scrobbler.DataContracts
{
	[DataContract]
	public class GetSessionResponse
	{
		[DataMember]
		public LastFMSession Session { get; set; }
	}
}
