/*
 * Copyright © 2014 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Papyrus.Core.Util
{

	internal static class PluginSerializer
	{

		/// <summary>
		/// Load a Plugin object from JSON string. Provide an existingCollection to resolve any partial records
		/// </summary>
		/// <param name="json">JSON string</param>
		/// <param name="loadRecords">True to load records, false to load only the header information.</param>
		/// <param name="existingCollection">Existing RecordCollection to resolve partial records from.</param>
		/// <returns></returns>
		public static Plugin FromJson(string json, bool loadRecords, RecordCollection existingCollection = null)
		{


			var jObj = JObject.Parse(Serialization.StripComments(json));

			// Load meta-data
			var p = new Plugin(jObj["Name"].Value<string>());

			if(jObj["Author"] != null)
				p.Author = jObj["Author"].Value<string>();

			if(jObj["Description"] != null)
				p.Description = jObj["Description"].Value<string>();

			if(jObj["Parents"] != null)
				p.InternalParents = jObj["Parents"].Values<string>().ToList();

			p.Records = null;
			p.IsLoaded = false;
			p.RecordJson = jObj["Records"].ToString();

			if (loadRecords) {
				LoadRecordsJson(p, existingCollection); 
				p.RefreshParents();
			}


			return p;

		}

		/// <summary>
		/// Load plugin Records. 
		/// </summary>
		/// <param name="plugin">Plugin to load</param>
		/// <param name="existingCollection">Existing RecordCollection resolve partial records from (or null)</param>
		public static void LoadRecordsJson(Plugin plugin, RecordCollection existingCollection)
		{

			if(plugin.IsLoaded)
				throw new InvalidOperationException("Plugin is already loaded");

			if(string.IsNullOrEmpty(plugin.RecordJson))
				throw new InvalidOperationException("Plugin has no JSON string");

			plugin.Records = (RecordCollectionSerializer.FromJson(plugin.RecordJson, existingCollection));
			plugin.IsLoaded = true;
			plugin.RecordJson = null;

		}

		public static string ToJson(Plugin plugin, RecordCollection parent = null)
		{

			var jObj = new JObject();

			jObj["Name"] = plugin.Name;
			jObj["Author"] = plugin.Author;
			jObj["Description"] = plugin.Description;

			plugin.RefreshParents();
			jObj["Parents"] = JArray.FromObject(plugin.Parents);

			jObj["Records"] = new JRaw(RecordCollectionSerializer.ToJson(plugin.Records, parent));

			return JToken.Parse(jObj.ToString(Formatting.None)).ToString(Formatting.Indented);

		}


	}

}
