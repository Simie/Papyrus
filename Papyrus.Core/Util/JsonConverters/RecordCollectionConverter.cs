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
using Newtonsoft.Json.Linq;

namespace Papyrus.Core.Util.JsonConverters
{
	class RecordCollectionConverter : Newtonsoft.Json.JsonConverter
	{

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			
			if(!(value is RecordCollection))
				throw new JsonSerializationException("Expected RecordCollection");

			var collection = value as RecordCollection;

			writer.WriteRaw(RecordCollectionSerializer.ToJson(collection));

		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{

			if (objectType != typeof(RecordCollection))
				return null;

			if (reader.TokenType == JsonToken.Null)
				return null;

			// Check for existing record collection, or create a new one
			//var collection = (existingValue is RecordCollection) ? (RecordCollection)existingValue : new RecordCollection();

			return RecordCollectionSerializer.FromJson(JArray.Load(reader).ToString());

		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(RecordCollection);
		}

	}
}
