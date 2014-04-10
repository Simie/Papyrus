using System;
using System.Collections.Generic;

namespace Papyrus.Core
{

	/// <summary>
	/// General interface for fetching records
	/// </summary>
	public interface IRecordDatabase
	{

		/// <summary>
		/// Get record with key
		/// </summary>
		/// <typeparam name="T">Record type</typeparam>
		/// <param name="key">Record key</param>
		/// <returns></returns>
		T GetRecord<T>(RecordKey key) where T : Record, new();

		/// <summary>
		/// Get record with key and type
		/// </summary>
		/// <param name="type">Record type</param>
		/// <param name="key">Record key</param>
		/// <returns></returns>
		Record GetRecord(Type type, RecordKey key);

		/// <summary>
		/// Get all records of type T
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		ICollection<T> GetRecords<T>() where T : Record;

		/// <summary>
		/// Get all records of type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		ICollection<Record> GetRecords(Type type);

		/// <summary>
		/// Get record from a reference.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="recordRef"></param>
		/// <param name="throwException">True to throw an exception if not found. Defaults to false</param>
		/// <returns></returns>
		T Get<T>(RecordRef<T> recordRef, bool throwException = false) where T : Record;

	}

}