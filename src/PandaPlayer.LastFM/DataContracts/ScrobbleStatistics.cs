using System.Runtime.Serialization;

namespace PandaPlayer.LastFM.DataContracts
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
