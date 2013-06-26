/*
 * Copyright © 2013 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Papyrus.Core.Util.JsonConverters
{
	class RecordRefCollectionConverter : Newtonsoft.Json.JsonConverter
	{

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{

			var type = value.GetType();

			if(!(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(RecordRefCollection<>)))
				throw new JsonSerializationException("Expected value to be a RecordRef<T>");

			var recordType = type.GetGenericArguments()[0];

			writer.WriteComment(string.Format("{0} Collection", recordType.Name));

			var collection = (IRecordRefCollection) value;

			serializer.Serialize(writer, collection.References);
			
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{

			if (!(objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(RecordRefCollection<>)))
				throw new JsonSerializationException("Expected value to be a RecordRefCollection<T>");

			var recordType = objectType.GetGenericArguments()[0];
			var recordRefType = typeof (RecordRef<>).MakeGenericType(recordType);

			// Deserialize as a list of RecordRef<T>
		

			var list = serializer.Deserialize(reader, typeof (List<>).MakeGenericType(recordRefType));

			var constructorArgument = typeof (IEnumerable<>).MakeGenericType(recordRefType);

			var constructor = objectType.GetConstructor(new [] { constructorArgument });

			if(constructor == null) 
				throw new JsonSerializationException("Could not find RecordRefCollection<> constructor");

			return constructor.Invoke(new [] { list });

		}

		public override bool CanConvert(Type objectType)
		{
			return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(RecordRefCollection<>);
		}

	}
}