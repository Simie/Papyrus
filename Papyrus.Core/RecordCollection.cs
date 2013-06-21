using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papyrus.Core
{

	/// <summary>
	/// A collection of RecordList objects with add/remove operations
	/// </summary>
	internal class RecordCollection
	{

		struct RecordList
		{

			public Type Type;
			public Dictionary<RecordKey, Record> Records;

		}

		private readonly Dictionary<Type,RecordList> _recordLists;

		public RecordCollection()
		{
			_recordLists = new Dictionary<Type, RecordList>();
		}

		/// <summary>
		/// Retrieve record from the collection. Throws exception if not found.
		/// </summary>
		/// <param name="recordType">Record Type</param>
		/// <param name="key">Record Key</param>
		/// <returns>Retrieved record</returns>
		public Record GetRecord(Type recordType, RecordKey key)
		{

			Record record;

			if(!TryGetRecord(recordType, key, out record))
				throw new KeyNotFoundException("No record with that key found.");

			return record;

		}

		public T GetRecord<T>(RecordKey key) where T : Record
		{
			return (T)GetRecord(typeof (T), key);
		}

		/// <summary>
		/// Attempt to retreive a record from this collection
		/// </summary>
		/// <param name="recordType">Record Type</param>
		/// <param name="key">Record Key</param>
		/// <param name="value">Will have the retrieved record assigned</param>
		/// <returns>True if the record was found, otherwise false</returns>
		public bool TryGetRecord(Type recordType, RecordKey key, out Record value)
		{

			if (!typeof(Record).IsAssignableFrom(recordType))
				throw new InvalidOperationException("Attempted to lookup a non-record type");

			RecordList list;

			if (!_recordLists.TryGetValue(recordType, out list)) {
				value = null;
				return false;
			}

			return list.Records.TryGetValue(key, out value);

		}

		public void AddRecord(RecordKey key, Record rec)
		{

			Type type = rec.GetType();

			RecordList recordList;

			// Check a record list for this record type exists
			if (!_recordLists.TryGetValue(type, out recordList)) {

				// Create it if not
				recordList = new RecordList() {
					Type = type,
					Records = new Dictionary<RecordKey, Record>()
				};
				_recordLists.Add(type, recordList);

			}

			recordList.Records.Add(key, rec);

		}

		/// <summary>
		/// Remove record with the given key
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <returns>True if the record was removed</returns>
		public bool RemoveRecord<T>(RecordKey key)
		{
			return RemoveRecord(typeof(T), key);
		}

		/// <summary>
		/// Remove the record with the given type and key
		/// </summary>
		/// <param name="recordType"></param>
		/// <param name="key"></param>
		/// <returns>True if the record was removed</returns>
		public bool RemoveRecord(Type recordType, RecordKey key)
		{
			
			RecordList recordList;

			// Check a record list for this record type exists
			if (!_recordLists.TryGetValue(recordType, out recordList))
				return false;

			return recordList.Records.Remove(key);

		}

	}

}
