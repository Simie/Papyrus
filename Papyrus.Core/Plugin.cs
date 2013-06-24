using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Papyrus.Core
{

	/// <summary>
	/// A collection of record lists, with add/remove operations
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public class Plugin
	{

		/// <summary>
		/// Load a plugin from a json string
		/// </summary>
		/// <param name="json"></param>
		/// <returns></returns>
		public static Plugin FromString(string json)
		{
			return JsonConvert.DeserializeObject<Plugin>(json, Util.Serialization.GetJsonSettings());
		}

		[JsonProperty]
		public string Name { get; private set; }

		[JsonProperty]
		internal RecordCollection Records { get; private set; }

		internal Plugin(string name)
		{

			Name = name;
			Records = new RecordCollection();

		}

		[JsonConstructor]
		Plugin() { }

		/// <summary>
		/// Get a key for the next record
		/// </summary>
		/// <param name="recordType"></param>
		/// <returns></returns>
		internal RecordKey NextKey(Type recordType)
		{

			var records = Records.GetRecords(recordType);

			var nextIndex = 0;

			if(records.Any())
				nextIndex = records.Max(p => p.InternalKey.Index)+1;

			return new RecordKey(nextIndex, Name);

		}

	}

}
