using System.Runtime.Serialization;

namespace MusicLibrary.LastFM.DataContracts
{
	[DataContract]
	public class GetTokenResponse
	{
		[DataMember]
		public string Token { get; set; }
	}
}
