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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Papyrus.Core
{

	/// <summary>
	/// A collection of record lists, with add/remove operations. Will defer loading of records until requested.
	/// </summary>
	public class Plugin
	{

		/// <summary>
		/// File extension for a plugin file
		/// </summary>
		public const string Extension = "jpp"; // json-papyrus-plugin

		/// <summary>
		/// Plugin name. Should never be changed after first set. Is used for record references.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Plugins that this plugin depends upon.
		/// </summary>
		public IList<string> Parents { get { return InternalParents.AsReadOnly(); } }

		#region Plugin Meta Data

		/// <summary>
		/// Author of the plugin (displayed in Papyrus Studio)
		/// </summary>
		public string Author { get; set; }

		/// <summary>
		/// Plugin description (displayed in Papyrus Studio)
		/// </summary>
		public string Description { get; set; }

		#endregion

		/// <summary>
		/// True if the records in this plugin have been loaded.
		/// </summary>
		public bool IsLoaded { get; internal set; }

		/// <summary>
		/// If plugin is loaded, a RecordCollection object containing all records in this plugin.
		/// </summary>
		internal RecordCollection Records { get; set; }

		/// <summary>
		/// If plugin is not loaded, this will contain the JSON string which should be parsed to load it.
		/// </summary>
		internal string RecordJson { get; set; }

		internal List<string> InternalParents = new List<string>(); 

		internal Plugin(string name)
		{

			if(!IsValidName(name))
				throw new ArgumentException("Plugin name is not in valid format. Check Plugin.IsValidName(string)", "name");

			Name = name;
			IsLoaded = true;
			Records = new RecordCollection();

		}

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
		/// Verify that the plugin list provided contains all the parents of this plugin.
		/// </summary>
		/// <param name="plugins"></param>
		/// <returns></returns>
		internal bool VerifyParents(IList<Plugin> plugins)
		{
			return InternalParents.All(parent => plugins.Any(p => p.Name == parent));
		}

		/// <summary>
		/// Scan records in this plugin to determine which plugins this plugin is child too
		/// </summary>
		internal void RefreshParents()
		{

			// Can't refresh if not loaded
			if (!IsLoaded)
				return;

			// Clear existing parent list
			InternalParents.Clear();

			// Fetch all records
			var records = Records.GetAllRecords();

			foreach (var record in records) {

				// If this record is overriding a parents record, add that parent to the list
				if(record.InternalKey.Plugin != Name && !InternalParents.Contains(record.InternalKey.Plugin))
					InternalParents.Add(record.InternalKey.Plugin);

				// Add any non-existing parents which have records referenced to the parent list
				InternalParents.AddRange(
					Util.RecordUtils.GetReferences(record)
						.Select(p => p.Key.Plugin).Distinct()
						.Where(p => !InternalParents.Contains(p) && p != Name && !string.IsNullOrEmpty(p)));

			}

			// Don't set self as a parent. Use while loop in case it has somehow been added more than once.
			while (InternalParents.Remove(Name)) { }
				
		} 

		/// <summary>
		/// Check that a string is a valid plugin name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static bool IsValidName(string name)
		{

			if (name == null)
				return false;

			if (name.Length < 2)
				return false;

			Regex rgx = new Regex("[^a-zA-Z0-9 -]");

			return !rgx.IsMatch(name);

		}

	}

}
