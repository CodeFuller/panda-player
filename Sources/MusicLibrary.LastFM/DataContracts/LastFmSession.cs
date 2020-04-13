using System.Runtime.Serialization;

namespace MusicLibrary.LastFM.DataContracts
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
