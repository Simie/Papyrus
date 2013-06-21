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

		public T GetRecord<T>(RecordKey key) where T : Record
		{
			throw new NotImplementedException();
		}

		public Record GetRecord(Type type, RecordKey key)
		{
			throw new NotImplementedException();
		}

		public ICollection<T> GetRecords<T>() where T : Record
		{
			throw new NotImplementedException();
		}

		public ICollection<Record> GetRecords(Type type)
		{
			throw new NotImplementedException();
		} 

	}

}
