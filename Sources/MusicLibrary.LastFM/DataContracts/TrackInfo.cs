using System.Runtime.Serialization;

namespace MusicLibrary.LastFM.DataContracts
{
	[DataContract]
	public class TrackInfo
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public int UserPlayCount { get; set; }
	}
}
