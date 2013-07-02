using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Papyrus.Core.Util.JsonConverters
{
	class ReadOnlyCollectionConverter : JsonConverter
	{

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{

			// Remove this converter to prevent infinite loop
			serializer.Converters.Remove(this);

			// Serialize as-is, it's just an IEnumerable
			serializer.Serialize(writer, value);

			// Add converter back
			serializer.Converters.Add(this);

		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			
			// Deserialize as a list, then pass that to the constructor

			var valueType = objectType.GetGenericArguments()[0];

			var list = serializer.Deserialize(reader, typeof (List<>).MakeGenericType(valueType));

			return Activator.CreateInstance(objectType, list);

		}

		public override bool CanConvert(Type objectType)
		{
			return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof (ReadOnlyCollection<>);
		}

	}
}
