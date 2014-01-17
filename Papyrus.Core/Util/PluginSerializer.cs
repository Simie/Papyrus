/*
 * Copyright © 2013 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using Newtonsoft.Json.Linq;

namespace Papyrus.Core.Util
{

	internal static class PluginSerializer
	{

		public static Plugin FromJson(string json, RecordCollection existingCollection = null)
		{


			var jObj = JObject.Parse(Serialization.StripComments(json));

			// Load meta-data
			var p = new Plugin(jObj["Name"].Value<string>());

			if(jObj["Author"] != null)
				p.Author = jObj["Author"].Value<string>();

			if(jObj["Description"] != null)
				p.Description = jObj["Description"].Value<string>();

			p.Records.Merge(RecordCollectionSerializer.FromJson(jObj["Records"].ToString(), existingCollection));

			p.RefreshParents();

			return p;

		}

		public static string ToJson(Plugin plugin, RecordCollection parent = null)
		{

			var jObj = new JObject();

			jObj["Name"] = plugin.Name;
			jObj["Author"] = plugin.Author;
			jObj["Description"] = plugin.Description;

			jObj["Records"] = new JRaw(RecordCollectionSerializer.ToJson(plugin.Records, parent));

			return jObj.ToString();

		}


	}

}
