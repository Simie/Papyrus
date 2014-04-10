/*
 * Copyright © 2014 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;
using System.Collections.Generic;
using Papyrus.Core.Util;
using System.Linq;
using System.Text;

namespace Papyrus.Core
{

	/// <summary>
	/// Compiles a collection of plugins into a database. Note that this database cannot be modified, the process of
	/// compiling the plugins does not maintain the plugin structure. Use this for production, and <c>PluginComposer</c> for composing
	/// a plugin.
	/// </summary>
	public sealed class RecordDatabase : IRecordDatabase
	{

		/// <summary>
		/// Composite collection of records from loaded plugins.
		/// </summary>
		private RecordCollection _internalCollection;

		public RecordDatabase()
		{
			_internalCollection = new RecordCollection();
		}

		/// <summary>
		/// Create a database, loading from the provided plugins
		/// </summary>
		/// <param name="plugins">List of plugins to composite into this database.</param>
		/// <param name="sort">Sort the list before appending (ensures that dependencies are loaded in order). Defaults to true.</param>
		public RecordDatabase(IList<Plugin> plugins, bool sort = true) : this()
		{

			IList<string> missing = new List<string>();

			// Check all parents are present
			if (!plugins.All(p => p.VerifyParents(plugins, missing))) {
				throw new MissingPluginException("Not all plugin parents can be resolved.", string.Join(", ", missing.ToArray()));
			}

			// Sort plugins
			if (sort) {
				plugins = PluginUtil.SortPluginList(plugins);
			}

			// Load plugins
			foreach (var plugin in plugins) {
				LoadPlugin(plugin);
			}

		}

		internal void LoadPlugin(Plugin plugin)
		{
			
			if(!plugin.IsLoaded)
				PluginSerializer.LoadRecordsJson(plugin, _internalCollection);

			// Merge plugin records into internal collection
			_internalCollection.Merge(plugin.Records);

		}

		/// <summary>
		/// Get record with key
		/// </summary>
		/// <typeparam name="T">Record type</typeparam>
		/// <param name="key">Record key</param>
		/// <returns></returns>
		public T GetRecord<T>(RecordKey key) where T : Record, new()
		{
			return _internalCollection.GetRecord<T>(key);
		}

		/// <summary>
		/// Get record with key and type
		/// </summary>
		/// <param name="type">Record type</param>
		/// <param name="key">Record key</param>
		/// <returns></returns>
		public Record GetRecord(Type type, RecordKey key)
		{
			return _internalCollection.GetRecord(type, key);
		}

		/// <summary>
		/// Get all records of type T
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public ICollection<T> GetRecords<T>() where T : Record
		{
			return _internalCollection.GetRecords<T>();
		}

		/// <summary>
		/// Get all records of type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public ICollection<Record> GetRecords(Type type)
		{
			return _internalCollection.GetRecords(type);
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
			Record rec;
			if(!_internalCollection.TryGetRecord(recordRef.ValueType, recordRef.Key, out rec) && throwException)
				throw new KeyNotFoundException("No record with key found");
			return rec as T;
		}

	}

}
