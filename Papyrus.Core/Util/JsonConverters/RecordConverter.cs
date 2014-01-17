/*
 * Copyright © 2014 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
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
using Newtonsoft.Json.Linq;

namespace Papyrus.Core.Util.JsonConverters
{
	class RecordConverter : Newtonsoft.Json.JsonConverter
	{

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{

			var record = (Record) value;

			writer.WriteRaw(RecordSerializer.ToJson(record));

		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{

			var record = objectType.IsInstanceOfType(existingValue) ? (Record)existingValue : (Record)Activator.CreateInstance(objectType);

			return RecordSerializer.FromJson(JObject.Load(reader).ToString(), objectType, record);

		}

		public override bool CanConvert(Type objectType)
		{
			return typeof (Record).IsAssignableFrom(objectType);
		}

	}
}
