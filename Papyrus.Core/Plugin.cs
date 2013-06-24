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

		public IList<string> Parents { get { return _parents.AsReadOnly(); } }
			
		[JsonProperty]
		internal RecordCollection Records { get; private set; }

		[JsonProperty("Parents")]
		private List<string> _parents = new List<string>(); 

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

		/// <summary>
		/// Scan records in this plugin to determine which plugins this plugin is child too
		/// </summary>
		internal void RefreshParents()
		{

			// Clear existing parent list
			_parents.Clear();

			// Fetch all records
			var records = Records.GetAllRecords();

			foreach (var record in records) {

				// If this record is overriding a parents record, add that parent to the list
				if(record.InternalKey.Plugin != Name)
					_parents.Add(record.InternalKey.Plugin);

				// Add any non-existing parents which have records referenced to the parent list
				_parents.AddRange(Util.RecordUtils.GetReferences(record).Select(p => p.Key.Plugin).Where(p => !string.IsNullOrEmpty(p)).Except(_parents));

			}

		} 

	}

}
