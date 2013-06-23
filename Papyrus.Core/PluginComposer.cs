using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

		/// <summary>
		/// Additional loaded plugins (read-only)
		/// </summary>
		private readonly List<Plugin> _pluginList;

		internal PluginComposer(Plugin plugin)
		{
			Plugin = plugin;
			_pluginList = new List<Plugin>();
		}

		internal PluginComposer(Plugin plugin, IEnumerable<Plugin> plugins)
		{
			Plugin = plugin;
			_pluginList = new List<Plugin>(plugins);
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
			
			if(!typeof(Record).IsAssignableFrom(type))
				throw new ArgumentException("Type is not decended from record");

			var record = (Record)Activator.CreateInstance(type);
			record.InternalKey = Plugin.NextKey(type);
			record.IsFrozen = true;

			Plugin.Records.AddRecord(record);

			return record;

		}

		/// <summary>
		/// Save changes to a record.
		/// </summary>
		/// <param name="record"></param>
		public void SaveRecord(Record record)
		{
			
		}

		public T GetRecord<T>(RecordKey key) where T : Record
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

		public ICollection<T> GetRecords<T>() where T : Record
		{
			return GetMergedCollection().GetRecords<T>();
		}

		public ICollection<Record> GetRecords(Type type)
		{
			return GetMergedCollection().GetRecords(type);
		}

		/// <summary>
		/// Create a RecordCollection with all dependencies and the primary plugin merged in order.
		/// </summary>
		/// <returns></returns>
		private RecordCollection GetMergedCollection()
		{

			var collection = new RecordCollection();

			// Merge all plugin record collections
			for (int i = 0; i < _pluginList.Count; i++) {
				collection.Merge(_pluginList[i].Records);
			}

			// Merge plugin records
			collection.Merge(Plugin.Records);

			return collection;

		}

	}

}
