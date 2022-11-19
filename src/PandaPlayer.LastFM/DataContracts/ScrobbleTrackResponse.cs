using System.Runtime.Serialization;

namespace PandaPlayer.LastFM.DataContracts
{
	[DataContract]
	public class ScrobbleTrackResponse
	{
		[DataMember]
		public ScrobblesInfo Scrobbles { get; set; }
	}
}
