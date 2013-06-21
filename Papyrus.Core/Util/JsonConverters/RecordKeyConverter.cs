using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Papyrus.Core.Util.JsonConverters
{
	class RecordKeyConverter : Newtonsoft.Json.JsonConverter
	{

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var key = (RecordKey) value;
			writer.WriteValue(key.ToString());
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{

			if (objectType != typeof (RecordKey))
				return null;

			var str = reader.Value.ToString();
			return RecordKey.FromString(str);

		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof (RecordKey);
		}

	}
}
