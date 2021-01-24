using System.Runtime.Serialization;
using MusicLibrary.LastFM.Internal;
using Newtonsoft.Json;

namespace MusicLibrary.LastFM.DataContracts
{
	[DataContract]
	public class CorrectableText
	{
		[DataMember(Name = "#text")]
		public string Text { get; set; }

		[DataMember]
		[JsonConverter(typeof(BooleanJsonConverter))]
		public bool Corrected { get; set; }
	}
}
