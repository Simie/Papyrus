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
	class RecordRefConverter : Newtonsoft.Json.JsonConverter
	{

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{

			var type = value.GetType();

			if(!(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(RecordRef<>)))
				throw new JsonSerializationException("Expected value to be a RecordRef<T>");

			var recordRef = value as IRecordRef;

			var key = value.GetType().GetProperty("Key").GetValue(value, null).ToString();


			if (recordRef.ValueType != recordRef.Type) {
				key += ", " + (recordRef.ValueType.FullName);
			}

			writer.WriteValue(key);

		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{

			if (!(objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(RecordRef<>)))
				throw new JsonSerializationException("Expected value to be a RecordRef<T>");

			var str = reader.Value.ToString();

			RecordKey key;
			Type valueType = null;

			// Check for polymorphic reference
			if (str.Contains(',')) {

				var split = str.Split(',');
				key = RecordKey.FromString(split[0]);
				valueType = ReflectionUtil.ResolveRecordType(split[1]);

			} else {

				key = RecordKey.FromString(str);

			}

			var constructor = objectType.GetConstructor(new [] { typeof (RecordKey), typeof(Type) });

			if(constructor == null) 
				throw new JsonSerializationException("Could not find RecordRef<> constructor");

			return constructor.Invoke(new object[] { key, valueType });

		}

		public override bool CanConvert(Type objectType)
		{
			return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(RecordRef<>);
		}

	}
}
