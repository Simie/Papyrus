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
	class ReadOnlyCollectionConverter : JsonConverter
	{

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{

			// Get internal list
			var internalList = (value.GetType().GetProperty("List", BindingFlags.Instance | BindingFlags.NonPublic)).GetValue(value, null);

			// Serialize list
			serializer.Serialize(writer, internalList);

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
