/*
 * Copyright © 2014 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Papyrus.Core.Util
{

	internal static class RecordSerializer
	{

		public static T FromJson<T>(string jsonString, T existingRecord = null) where T : Record
		{
			return (T) FromJson(jsonString, typeof (T), existingRecord);
		}

		public static Record FromJson(string jsonString, Type recordType, Record existingRecord = null)
		{

			// Use existing record instance if provided, or create new.
			if (existingRecord == null)
				existingRecord = (Record)Activator.CreateInstance(recordType);

			var serializer = Serialization.GetJsonSerializer();
			var validProperties = RecordReflectionUtil.GetProperties(recordType);

			var jObj = JObject.Parse(jsonString);

			// Iterate over properties in record json
			foreach (var jProp in jObj) {

				switch (jProp.Key) {
						
					case "Key":
						existingRecord.InternalKey = RecordKey.FromString(jProp.Value.Value<string>());
						break;

					default:

						var found = false;

						// Check for valid properties
						foreach (var p in validProperties) {

							if (p.Name != jProp.Key)
								continue;

							// Deserialize property and set property value
							existingRecord.SetProperty(p.Name, serializer.Deserialize(jProp.Value.CreateReader(), p.PropertyType));

							found = true;
							break;

						}

						if(!found)
							Debug.WriteLine("Property {0} is not a valid property", jProp.Key);

						break;

				}

			}

			return existingRecord;

		}

		/// <summary>
		/// Convert record into a JObject, optionally comparing propery values to parentRecord and only saving differences.
		/// </summary>
		/// <param name="record">Record to convert to JSON</param>
		/// <param name="parentRecord">If not null, compare all property values in record to parentRecord, and only save differences.</param>
		/// <returns></returns>
		public static string ToJson(Record record, Record parentRecord = null)
		{

			if (parentRecord != null && record.GetType() != parentRecord.GetType())
				throw new ArgumentException("record and parentRecord must be the same type", "parentRecord");

			var serializer = Serialization.GetJsonSerializer();
			var jObj = new JObject();

			jObj["Key"] = record.InternalKey.ToString();

			// If using parentRecord, only serialize differences
			if (parentRecord != null) {

				var diff = RecordDiffUtil.Diff(parentRecord, record);

				foreach (var d in diff) {

					jObj.Add(d.Property.Name, d.NewValue == null ? null : JToken.FromObject(d.NewValue, serializer));

				}

			} else {

				var validProperties = RecordReflectionUtil.GetProperties(record.GetType());

				foreach (var p in validProperties) {

					var value = p.GetValue(record, null);

					jObj.Add(p.Name, value == null ? null : (JToken.FromObject(value, serializer)));

				}

			}

			return jObj.ToString();

		}

	}

}
