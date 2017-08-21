using System.Runtime.Serialization;

namespace CF.MusicLibrary.PandaPlayer.Scrobbler.DataContracts
{
	[DataContract]
	public class LastFMSession
	{
		[DataMember]
		public int Subscriber { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Key { get; set; }
	}
}
