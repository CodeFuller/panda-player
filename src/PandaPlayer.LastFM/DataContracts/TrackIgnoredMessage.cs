using System.Runtime.Serialization;

namespace PandaPlayer.LastFM.DataContracts
{
	[DataContract]
	public class TrackIgnoredMessage
	{
		[DataMember(Name = "#text")]
		public string Text { get; set; }

		[DataMember]
		public int Code { get; set; }
	}
}
