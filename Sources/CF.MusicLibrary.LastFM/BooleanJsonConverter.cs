using System;
using Newtonsoft.Json;

namespace CF.MusicLibrary.LastFM
{
#pragma warning disable CA1812 // The class is instantiated by DI container - The class is instantiated by Json.NET
	internal class BooleanJsonConverter : JsonConverter
#pragma warning restore CA1812 // The class is instantiated by DI container
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
