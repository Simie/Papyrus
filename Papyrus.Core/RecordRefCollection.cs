/*
 * Copyright © 2014 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;

namespace Papyrus.Core
{

	/// <summary>
	/// Non-generic interface for a RecordRefCollection
	/// </summary>
	public interface IRecordRefCollection : IEnumerable
	{

		/// <summary>
		/// Type object of the record type being references by objects in this list
		/// </summary>
		Type RecordType { get; }

		/// <summary>
		/// Read-only list of record references
		/// </summary>
		IList<IRecordRef> References { get; } 

	}

	/// <summary>
	/// A collection of RecordRef objects
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[JsonObject()]
	[JsonConverter(typeof(Util.JsonConverters.RecordRefCollectionConverter))]
	public struct RecordRefCollection<T> : IRecordRefCollection, IEquatable<RecordRefCollection<T>>, ICollection<RecordRef<T>> where T : Record
	{

		private readonly List<RecordRef<T>> _internalList;
			
		/// <summary>
		/// Type object of the record type being references by objects in this list
		/// </summary>
		[JsonIgnore]
		public Type RecordType { get { return typeof (T); } }

		/// <summary>
		/// Read-only list of record references
		/// </summary>
		public IList<RecordRef<T>> References
		{
			get
			{

				if(_internalList == null)
					return new RecordRef<T>[0];

				return _internalList.AsReadOnly();

			}
		}

		/// <summary>
		/// Interface implementation of References property
		/// </summary>
		IList<IRecordRef> IRecordRefCollection.References
		{
			get { return References.Cast<IRecordRef>().ToArray(); }
		} 

		/// <summary>
		/// Create a new read-only RecordRefCollection with the provided RecordRef objects
		/// </summary>
		/// <param name="recordRefs"></param>
		public RecordRefCollection(IEnumerable<RecordRef<T>> recordRefs)
		{
			_internalList = new List<RecordRef<T>>(recordRefs);
		}

		/// <summary>
		/// Enumerate this collection
		/// </summary>
		/// <returns></returns>
		public IEnumerator<RecordRef<T>> GetEnumerator()
		{
			return _internalList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}


		/// <summary>
		/// Check elements of both collections for equality
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(RecordRefCollection<T> other)
		{

			if (other._internalList == null && _internalList == null)
				return true;

			if (other._internalList == null || _internalList == null)
				return false;

			if (_internalList.Count != other._internalList.Count)
				return false;

			for (var i = 0; i < _internalList.Count; i++) {
				if (!_internalList[i].Equals(other._internalList[i]))
					return false;
			}

			return true;

		}

		/// <summary>
		/// Check if collection is equal to obj
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			return obj is RecordRefCollection<T> && Equals((RecordRefCollection<T>) obj);
		}

		/// <summary>
		/// Get a hash-code to identify this collection
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return (_internalList != null ? _internalList.GetHashCode() : 0);
		}

		/// <summary>
		/// Compare two record ref collections
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator ==(RecordRefCollection<T> left, RecordRefCollection<T> right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Compare two record ref collections
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator !=(RecordRefCollection<T> left, RecordRefCollection<T> right)
		{
			return !left.Equals(right);
		}

		/// <summary>
		/// Create a string representation of this RecordRefCollection
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{

			return string.Format("RecordRefCollection<{0}> ({1})", RecordType.Name,
				string.Join(", ", References.Select(p => p.ToString()).ToArray()));

		}

		#region ICollection

		/// <summary>
		/// Add item to collection (not supported)
		/// </summary>
		/// <param name="item"></param>
		public void Add(RecordRef<T> item)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Clear collection (not supported)
		/// </summary>
		public void Clear()
		{
			throw new NotSupportedException();	
		}

		/// <summary>
		/// Does this collection contain item
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(RecordRef<T> item)
		{
			return References.Contains(item);
		}

		/// <summary>
		/// Copy elements from collection to array
		/// </summary>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		public void CopyTo(RecordRef<T>[] array, int arrayIndex)
		{
			References.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Remove item from collection. Not Supported
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		/// <exception cref="NotSupportedException"></exception>
		public bool Remove(RecordRef<T> item)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Number of items in this collection
		/// </summary>
		public int Count { get { return References.Count; } }

		/// <summary>
		/// Yes
		/// </summary>
		public bool IsReadOnly { get { return true; } }

		#endregion

	}

}
