using System.Runtime.Serialization;

namespace PandaPlayer.LastFM.DataContracts
{
	[DataContract]
	public class TrackProcessingInfo
	{
		[DataMember]
		public CorrectableText Artist { get; set; }

		[DataMember]
		public CorrectableText AlbumArtist { get; set; }

		[DataMember]
		public CorrectableText Album { get; set; }

		[DataMember]
		public CorrectableText Track { get; set; }

		[DataMember]
		public TrackIgnoredMessage IgnoredMessage { get; set; }
	}
}
