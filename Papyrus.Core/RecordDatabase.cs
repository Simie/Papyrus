﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papyrus.Core
{

	/// <summary>
	/// Compiles a collection of plugins into a database. Note that this database cannot be modified, the process of
	/// compiling the plugins does not maintain the plugin structure. Use this for production, and <c>MutableRecordDatabase</c> for composing
	/// a plugin.
	/// </summary>
	public sealed class RecordDatabase
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
			


		}

		public void LoadPlugin(Plugin plugin)
		{
			
			// Merge plugin records into internal collection
			_internalCollection.Merge(plugin.Records);

		}

		public T GetRecord<T>(RecordKey key) where T : Record
		{
			return _internalCollection.GetRecord<T>(key);
		}

		public Record GetRecord(Type type, RecordKey key)
		{
			return _internalCollection.GetRecord(type, key);
		}

		public ICollection<T> GetRecords<T>() where T : Record
		{
			return _internalCollection.GetRecords<T>();
		}

		public ICollection<Record> GetRecords(Type type)
		{
			return _internalCollection.GetRecords(type);
		} 

	}

}
