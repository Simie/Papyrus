﻿/*
 * Copyright © 2014 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Papyrus.Core
{

	/// <summary>
	/// A collection of RecordList objects with add/remove operations
	/// </summary>
	[JsonConverter(typeof(Util.JsonConverters.RecordCollectionConverter))]
	internal class RecordCollection
	{

		internal struct RecordList
		{

			public Dictionary<RecordKey, Record> Records;

			public RecordList(Dictionary<RecordKey, Record> records) : this()
			{
				Records = records;
			}

		}

		internal readonly Dictionary<Type,RecordList> RecordLists;

		public RecordCollection()
		{
			RecordLists = new Dictionary<Type, RecordList>();
		}

		/// <summary>
		/// Create a new RecordCollection with the contents of another
		/// </summary>
		/// <param name="other"></param>
		public RecordCollection(RecordCollection other) : this()
		{
			Merge(other);
		}

		/// <summary>
		/// Retrieve record from the collection. Throws exception if not found.
		/// </summary>
		/// <param name="recordType">Record Type</param>
		/// <param name="key">Record Key</param>
		/// <returns>Retrieved record</returns>
		public Record GetRecord(Type recordType, RecordKey key)
		{

			if(recordType.IsAbstract)
				throw new ArgumentException("recordType is abstract", "recordType");

			Record record;

			if(!TryGetRecord(recordType, key, out record))
				throw new KeyNotFoundException("No record with that key found.");

			return record;

		}

		public T GetRecord<T>(RecordKey key) where T : Record, new()
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

			if (!recordType.IsSubclassOf(typeof(Record)))
				throw new InvalidOperationException("Attempted to lookup a non-record type");

			RecordList list;

			if (!RecordLists.TryGetValue(recordType, out list)) {
				value = null;
				return false;
			}

			return list.Records.TryGetValue(key, out value);

		}

		/// <summary>
		/// Add a record to the collection.
		/// </summary>
		/// <param name="record"></param>
		/// <param name="overwrite">True to overwrite existing record with duplicate key</param>
		public void AddRecord(Record record, bool overwrite = false)
		{

			if (record.Key == RecordKey.Identity)
				throw new ArgumentException("Record has no Key set");

			// Freeze record upon entering a collection
			record.IsFrozen = true;

			Type type = record.GetType();

			RecordList recordList;

			// Check a record list for this record type exists
			if (!RecordLists.TryGetValue(type, out recordList)) {

				// Create it if not
				recordList = new RecordList() {
					Records = new Dictionary<RecordKey, Record>(RecordKey.Comparer)
				};
				RecordLists.Add(type, recordList);

			}

			if(overwrite)
				recordList.Records[record.Key] = record;
			else
				recordList.Records.Add(record.Key, record);

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
			if (!RecordLists.TryGetValue(recordType, out recordList))
				return false;

			return recordList.Records.Remove(key);

		}

		/// <summary>
		/// Get all the records of a given type
		/// </summary>
		/// <returns></returns>
		public ICollection<T> GetRecords<T>() where T : Record
		{

			return (GetRecords(typeof (T)).Cast<T>().ToArray());

		} 

		/// <summary>
		/// Get all the records of a given type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public ICollection<Record> GetRecords(Type type)
		{

			if (type.IsAbstract)
				return GetRecordsTree(type);

			if(!RecordLists.ContainsKey(type))
				return new Record[0];

			return RecordLists[type].Records.Values;

		} 

		/// <summary>
		/// Get all records descending from type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private ICollection<Record> GetRecordsTree(Type type)
		{
			
			List<Record> retList = new List<Record>();

			foreach (var recordList in RecordLists) {
				
				if(recordList.Key.IsSubclassOf(type))
					retList.AddRange(recordList.Value.Records.Select(p => p.Value));

			}

			return retList;

		} 

		/// <summary>
		/// Get every record in this collection
		/// </summary>
		/// <returns></returns>
		public ICollection<Record> GetAllRecords()
		{
			return RecordLists.SelectMany(p => p.Value.Records.Values).ToList();
		} 

		/// <summary>
		/// Merge the other collection into this. Overwrites any overlapping records with the other collection's copy.
		/// </summary>
		/// <param name="other"></param>
		public void Merge(RecordCollection other)
		{

			foreach (var list in other.RecordLists) {

				var listType = list.Key;

				// Check for existing record list of that type
				if (!RecordLists.ContainsKey(listType)) {

					// If this collection doesn't have one, copy the other collections list wholesale.
					RecordLists.Add(listType, new RecordList(new Dictionary<RecordKey, Record>(list.Value.Records, RecordKey.Comparer)));
					continue;

				}

				var ourList = RecordLists[listType];

				// Iterate over records in the other collections list and add/replace records in this collections list
				foreach (var record in list.Value.Records) {

					ourList.Records[record.Key] = record.Value;

				}

			}

		}

	}

}
