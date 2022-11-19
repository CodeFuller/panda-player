using System;
using Newtonsoft.Json;

namespace PandaPlayer.LastFM.Internal
{
	internal class BooleanJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(bool);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader == null)
			{
				throw new ArgumentNullException(nameof(reader));
			}

			switch (reader.Value.ToString().Trim())
			{
				case "0":
					return false;

				case "1":
					return true;
			}

			return new JsonSerializer().Deserialize(reader, objectType);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}
