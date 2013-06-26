/*
 * Copyright © 2013 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Papyrus.Core.Util.JsonConverters
{
	class RecordCollectionConverter : Newtonsoft.Json.JsonConverter
	{

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			
			if(!(value is RecordCollection))
				throw new JsonSerializationException("Expected RecordCollection");

			var collection = value as RecordCollection;

			writer.WriteStartObject();

			foreach (var list in collection.RecordLists) {
				
				// Write record type name this list contains
				writer.WritePropertyName(list.Key.FullName);

				// Write records contained in the list

				writer.WriteStartArray();

				foreach (var record in list.Value.Records) {
					serializer.Serialize(writer, record.Value);
				}

				writer.WriteEndArray();

			}

			writer.WriteEndObject();

		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{

			if (objectType != typeof(RecordCollection))
				return null;

			if (reader.TokenType == JsonToken.Null)
				return null;

			// Read past StartObject
			reader.Read();

			// Check for existing record collection, or create a new one
			var collection = (existingValue is RecordCollection) ? (RecordCollection)existingValue : new RecordCollection();


			bool finished = false;

			do {
				switch (reader.TokenType) {

					case JsonToken.PropertyName: {

						var typeString = reader.Value.ToString();
						var type = ReflectionUtil.ResolveRecordType(typeString);

						if (type == null || !typeof(Record).IsAssignableFrom(type))
							throw new JsonSerializationException("Unable to resolve type");

						// Read past PropertyName
						reader.Read();

						var records = (IList)serializer.Deserialize(reader, typeof (List<>).MakeGenericType(type));

						foreach (var record in records) {
							collection.AddRecord((Record)record);
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

			if (!finished)
				throw new JsonSerializationException("Unexpected end when deserializing object");

			/*if (reader.TokenType != JsonToken.StartObject)
				throw new JsonSerializationException("Expected StartObject token");

			reader.Read();

			while (reader.TokenType != JsonToken.EndObject) {

				if(reader.TokenType != JsonToken.PropertyName)
					throw new JsonSerializationException("Expected PropertyName token");

				var typeString = reader.Value.ToString();

				var type = ReflectionUtil.ResolveRecordType(typeString);

				if(type == null || !typeof(Record).IsAssignableFrom(type))
					throw new JsonSerializationException("Unable to resolve type");

				reader.Read(); // Read start array
				reader.Read(); // Read first entry
				while (reader.TokenType != JsonToken.EndArray) {

					// Read record
					var record = (Record)serializer.Deserialize(reader, type);
					collection.AddRecord(record);

					reader.Read();

				}

				reader.Read();

			}*/

			return collection;

		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(RecordCollection);
		}

	}
}
