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
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Papyrus.Core.Util.JsonConverters;

namespace Papyrus.Core.Util
{

	internal static class RecordCollectionSerializer
	{

		/// <summary>
		/// Deserialize record collection,using existingCollection to resolve partial records.
		/// </summary>
		/// <param name="jsonString">JSON string</param>
		/// <param name="existingCollection">Existing record collection to resolve partial records from</param>
		/// <returns></returns>
		public static RecordCollection FromJson(string jsonString, RecordCollection existingCollection = null)
		{

			// Parse JObject from json string
			var jObj = JObject.Parse(jsonString);

			var col = new RecordCollection();

			// Iterate over RecordLists
			foreach (var recordList in jObj) {

				var typeName = recordList.Key;

				var recordType = ReflectionUtil.ResolveRecordType(typeName);

				if(recordType == null)
					throw new Exception("Unknown record type " + typeName);

				// Iterate over each record in list
				foreach (var record in recordList.Value) {

					var key = record["Key"];

					if(key == null)
						throw new JsonSerializationException("Expected Key property");

					var recordKey = RecordKey.FromString(key.Value<string>());

					Record r = null;

					// Check for existing record with key
					if (existingCollection != null && existingCollection.TryGetRecord(recordType, recordKey, out r)) {

						// Make a copy, so it's not modifying the contents the existing record collection
						r = r.Clone();

					}

					col.AddRecord(RecordSerializer.FromJson(record.ToString(), recordType, r));

				}

			}

			return col;

		}

		/// <summary>
		/// Convert RecordCollection to json object, optionally using parentCollection to allow partial record serialization
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="parentCollection"></param>
		/// <returns></returns>
		public static string ToJson(RecordCollection collection, RecordCollection parentCollection = null)
		{

			var jObj = new JObject();

			foreach (var recordList in collection.RecordLists) {

				var typeName = recordList.Key.FullName;
				var jArr = new JArray();

				foreach (var record in recordList.Value.Records) {

					Record parentRecord = null;
					
					if(parentCollection != null)
						parentCollection.TryGetRecord(recordList.Key, record.Key, out parentRecord);

					jArr.Add(new JRaw(RecordSerializer.ToJson(record.Value, parentRecord)));

				}

				jObj[typeName] = jArr;

			}

			return jObj.ToString();

		}

	}
}
