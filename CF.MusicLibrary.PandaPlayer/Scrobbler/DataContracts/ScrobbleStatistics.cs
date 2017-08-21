using System.Runtime.Serialization;

namespace CF.MusicLibrary.PandaPlayer.Scrobbler.DataContracts
{
	[DataContract]
	public class ScrobbleStatistics
	{
		[DataMember]
		public int Accepted { get; set; }

		[DataMember]
		public int Ignored { get; set; }
	}
}
