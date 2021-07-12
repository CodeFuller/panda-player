using System.Runtime.Serialization;

namespace PandaPlayer.LastFM.DataContracts
{
	[DataContract]
	public class ScrobblesInfo
	{
		[DataMember(Name = "@attr")]
		public ScrobbleStatistics Statistics { get; set; }

		[DataMember(Name = "Scrobble")]
		public ScrobbleInfo Scrobble { get; set; }
	}
}
