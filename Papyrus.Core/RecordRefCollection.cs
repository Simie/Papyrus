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
	public struct RecordRefCollection<T> : IRecordRefCollection, IEnumerable<RecordRef<T>>, IEquatable<RecordRefCollection<T>> where T : Record
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

		public IEnumerator<RecordRef<T>> GetEnumerator()
		{
			return _internalList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}


		public bool Equals(RecordRefCollection<T> other)
		{
			return References.SequenceEqual(other.References);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			return obj is RecordRefCollection<T> && Equals((RecordRefCollection<T>) obj);
		}

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

	}

}
