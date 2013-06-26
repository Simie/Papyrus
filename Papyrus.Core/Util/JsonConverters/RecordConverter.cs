/*
 * Copyright © 2013 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Papyrus.Core.Util.JsonConverters
{
	class RecordConverter : Newtonsoft.Json.JsonConverter
	{

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{

			var type = value.GetType();

			var record = (Record) value;

			writer.WriteStartObject();

			// Write record key
			writer.WritePropertyName("Key");
			serializer.Serialize(writer, record.InternalKey);

			// Get writable properties
			var properties = RecordReflectionUtil.GetProperties(type);

			foreach (var prop in properties) {
				
				writer.WritePropertyName(prop.Name);
				serializer.Serialize(writer, prop.GetValue(record, null));

			}

			writer.WriteEndObject();

		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{

			var record = objectType.IsInstanceOfType(existingValue) ? (Record)existingValue : (Record)Activator.CreateInstance(objectType);

			var validProperties = RecordReflectionUtil.GetProperties(objectType);

			bool finished = false;

			// Read past StartObject
			reader.Read();

			do {
				switch (reader.TokenType) {

					case JsonToken.PropertyName: {

						var propName = reader.Value.ToString();

						reader.Read();

						if (propName == "Key") {

							record.InternalKey = serializer.Deserialize<RecordKey>(reader);

						} else {

							var property = validProperties.FirstOrDefault(p => p.Name == propName);

							if (property != null) {
								record.SetProperty(propName, serializer.Deserialize(reader, property.PropertyType));
							}

						}

						break;
					}

					case JsonToken.EndObject:
						finished = true;
						break;
					case JsonToken.Comment:
						// ignore
						break;
					default:
						throw new JsonSerializationException("Unexpected token when deserializing object: " + reader.TokenType);
				}
			} while (!finished && reader.Read());

			if(!finished)
				throw new JsonSerializationException("Unexpected end when deserializing object");

			return record;

		}

		public override bool CanConvert(Type objectType)
		{
			return typeof (Record).IsAssignableFrom(objectType);
		}

	}
}
