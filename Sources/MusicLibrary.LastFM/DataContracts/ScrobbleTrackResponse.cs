using System.Runtime.Serialization;

namespace MusicLibrary.LastFM.DataContracts
{
	[DataContract]
	public class ScrobbleTrackResponse
	{
		[DataMember]
		public ScrobblesInfo Scrobbles { get; set; }
	}
}
