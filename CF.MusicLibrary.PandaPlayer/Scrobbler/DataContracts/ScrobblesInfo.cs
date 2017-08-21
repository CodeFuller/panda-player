using System.Runtime.Serialization;

namespace CF.MusicLibrary.PandaPlayer.Scrobbler.DataContracts
{
	[DataContract]
	public class ScrobblesInfo
	{
		[DataMember(Name = "@attr")]
		public ScrobbleStatistics Statistics { get; set; }

		[DataMember(Name = "Scrobble")]
		//	CF TEMP: Extend for arrays
		public ScrobbleInfo Scrobble { get; set; }
	}
}
