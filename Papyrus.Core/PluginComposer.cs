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
using Papyrus.Core.Util;

namespace Papyrus.Core
{

	/// <summary>
	/// Loads a selection of plugins and allows edit operations. The resulting changes can be saved
	/// to a new plugin. Slow read performance compared to <c>RecordDatabase</c>.
	/// </summary>
	public sealed class PluginComposer
	{

		#region Static Factory Methods

		/// <summary>
		/// Creates a blank plugin
		/// </summary>
		/// <param name="name">Name of the plugin to create.</param>
		public static PluginComposer CreateBlank(string name)
		{
			return new PluginComposer(new Plugin(name));
		}

		/// <summary>
		/// Create a new child plugin with the provided plugins as parents
		/// </summary>
		/// <param name="name">Name of the plugin to create</param>
		/// <param name="dependencies">List of plugins to load</param>
		/// <returns></returns>
		public static PluginComposer CreateChild(string name, IList<Plugin> dependencies)
		{
			return new PluginComposer(new Plugin(name), dependencies);
		}

		/// <summary>
		/// Edit a plugin. Provide a list of dependencies to load from.
		/// </summary>
		/// <param name="plugin">Plugin to edit</param>
		/// <param name="dependencies">List of plugins to load dependencies from</param>
		/// <returns></returns>
		public static PluginComposer EditPlugin(Plugin plugin, IList<Plugin> dependencies)
		{
			return new PluginComposer(plugin, dependencies);
		}

		#endregion

		/// <summary>
		/// Plugin being composed/edited
		/// </summary>
		public Plugin Plugin { get; private set; }

		public bool NeedSaving { get; private set; }

		/// <summary>
		/// Additional loaded plugins (read-only)
		/// </summary>
		private readonly IList<Plugin> _pluginList;

		/// <summary>
		/// Simple event when an operation should cause a record list refresh
		/// </summary>
		public event EventHandler RecordListChanged;

		PluginComposer(Plugin plugin)
		{
			Plugin = plugin;
			_pluginList = new List<Plugin>();
			Load();
		}

		PluginComposer(Plugin plugin, IList<Plugin> plugins, bool sort = true)
		{

			Plugin = plugin;
			_pluginList = sort ? PluginUtil.SortPluginList(plugins) : new List<Plugin>(plugins);

			if(!Plugin.VerifyParents(_pluginList) || !_pluginList.All(p => p.VerifyParents(_pluginList)))
				throw new MissingPluginException("Missing parent plugins", "Unknown");

			Load();

		}

		/// <summary>
		/// Ensure all plugins are loaded
		/// </summary>
		private void Load()
		{

			// Create a RecordCollection to resolve all partial records from
			var records = new RecordCollection();

			// Load all parent plugins (in order)
			foreach (var plugin in _pluginList) {

				if (!plugin.IsLoaded) {
					PluginSerializer.LoadRecordsJson(plugin, records);
				}

				records.Merge(plugin.Records);

			}

			// Load main plugin, using the accumulated record collection to resolve partial records
			if(!Plugin.IsLoaded)
				PluginSerializer.LoadRecordsJson(Plugin, records);

		}

		/// <summary>
		/// Thin wrapper around PluginLoader.SavePlugin, sets NeedSaving when complete.
		/// </summary>
		/// <param name="directory"></param>
		public void SavePlugin(string directory)
		{
			PluginLoader.SavePlugin(Plugin, directory, GetParentCollection());
			NeedSaving = false;
		}

		/// <summary>
		/// Create a new record and return an editable copy.
		/// </summary>
		/// <typeparam name="T">Record type</typeparam>
		/// <returns>Editable record copy</returns>
		public T CreateRecord<T>() where T : Record, new()
		{
			return (T)CreateRecord(typeof (T));
		}

		/// <summary>
		/// Create a new record and add it to the database. Returns an editable
		/// copy
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public Record CreateRecord(Type type)
		{
			
			if(!type.IsSubclassOf(typeof(Record)))
				throw new ArgumentException("Type is not subclass of Record", "type");

			if(type.IsAbstract)
				throw new ArgumentException("Type is abstract", "type");

			// Create new record of type
			var record = (Record)Activator.CreateInstance(type);

			// Assign next key
			record.InternalKey = Plugin.NextKey(type);

			// Set default editor ID
			record.SetProperty(() => record.EditorID,
			                   string.Format("{0}_{1}", type.Name.ToLower(), record.InternalKey.Index.ToString()));

			// Add to plugin collection
			Plugin.Records.AddRecord(record);

			OnRecordListChanged();
			NeedSaving = true;

			// Return editable clone
			return Util.RecordReflectionUtil.Clone(record);

		}

		/// <summary>
		/// Save changes to a record.
		/// </summary>
		/// <param name="record"></param>
		public void SaveRecord(Record record)
		{

			Record existing;

			NeedSaving = true; // TODO: Check diff before setting NeedSaving

			// Check if this record exists in the current plugin
			if (Plugin.Records.TryGetRecord(record.GetType(), record.Key, out existing)) {
				
				// Copy new values to existing record
				existing.IsFrozen = false;
				Util.RecordReflectionUtil.Populate(record, existing);
				existing.IsFrozen = true;

				return;

			}

			// Else create a clone and add it to this plugin
			var ourClone = Util.RecordReflectionUtil.Clone(record);
			ourClone.IsFrozen = true;

			// Add record to active plugin
			Plugin.Records.AddRecord(ourClone);

			// Editor record list needs updating as a result
			OnRecordListChanged();

		}

		public T GetRecord<T>(RecordKey key) where T : Record, new()
		{
			return (T) GetRecord(typeof (T), key);
		}

		public Record GetRecord(Type type, RecordKey key)
		{

			// Check if the active plugin contains this key
			Record record;

			if (Plugin.Records.TryGetRecord(type, key, out record))
				return record;

			// Check the dependency list (in reverse) for the key
			for (int i = _pluginList.Count - 1; i >= 0; i--) {

				if (_pluginList[i].Records.TryGetRecord(type, key, out record))
					return record;

			}

			throw new KeyNotFoundException("Record with key not found");

		}

		/// <summary>
		/// Get an editable copy of a record. SaveRecord must be called with this copy
		/// to save changes.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		public T GetEditableRecord<T>(RecordKey key) where T : Record
		{
			return (T)GetEditableRecord(typeof (T), key);
		}

		/// <summary>
		/// Get an editable copy of a record. SaveRecord must be called with this copy
		/// to save changes.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		public Record GetEditableRecord(Type type, RecordKey key)
		{

			var record = GetRecord(type, key);

			return Util.RecordReflectionUtil.Clone(record);

		}

		public ICollection<T> GetRecords<T>() where T : Record
		{
			return GetMergedCollection().GetRecords<T>();
		}

		public ICollection<Record> GetRecords(Type type)
		{
			return GetMergedCollection().GetRecords(type);
		}

		/// <summary>
		/// Get record from a reference.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="recordRef"></param>
		/// <param name="throwException">True to throw an exception if not found. Defaults to false</param>
		/// <returns></returns>
		public T Get<T>(RecordRef<T> recordRef, bool throwException = false) where T : Record
		{
			return Get((IRecordRef)recordRef, throwException) as T;
		}
	
		/// <summary>
		/// Get record from a reference.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="recordRef"></param>
		/// <param name="throwException">True to throw an exception if not found. Defaults to false</param>
		/// <returns></returns>
		public Record Get(IRecordRef recordRef, bool throwException = false)
		{
			Record rec;
			if (!GetMergedCollection().TryGetRecord(recordRef.ValueType, recordRef.Key, out rec) && throwException)
				throw new KeyNotFoundException("No record with key found");
			return rec;
		}

		/// <summary>
		/// Create a RecordCollection with all parent plugin records
		/// </summary>
		/// <returns></returns>
		private RecordCollection GetParentCollection()
		{

			var collection = new RecordCollection();

			// Merge all plugin record collections
			for (int i = 0; i < _pluginList.Count; i++) {
				collection.Merge(_pluginList[i].Records);
			}

			return collection;

		}

		/// <summary>
		/// Create a RecordCollection with all dependencies and the primary plugin merged in order.
		/// </summary>
		/// <returns></returns>
		private RecordCollection GetMergedCollection()
		{

			var collection = GetParentCollection();

			// Merge plugin records
			collection.Merge(Plugin.Records);

			return collection;

		}

		private void OnRecordListChanged()
		{

			EventHandler handler = RecordListChanged;

			if (handler != null)
				handler(this, EventArgs.Empty);

		}

	}
}
