using System.Runtime.Serialization;

namespace CF.MusicLibrary.LastFM.DataContracts
{
	[DataContract]
	public class ScrobbleTrackResponse
	{
		[DataMember]
		public ScrobblesInfo Scrobbles { get; set; }
	}
}
