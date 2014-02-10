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
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Papyrus.Core.Util;

namespace Papyrus.Core
{

	/// <summary>
	/// A struct-based Read Only collection for Papyrus Records
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[JsonConverter(typeof(Util.JsonConverters.ReadOnlyCollectionConverter))]
	public struct ReadOnlyCollection<T> : IList<T>, ICollection, IEquatable<ReadOnlyCollection<T>>
	{

		private PapyrusList<T> _internalList;

		/// <summary>
		/// Access the collection
		/// </summary>
		private IList<T> List {

			get {

				if(_internalList == null) _internalList = new PapyrusList<T>(0);
				return _internalList;

			}

		}

		/// <summary>
		/// Create a new ReadOnlyCollection with the specified items
		/// </summary>
		/// <param name="items"></param>
		public ReadOnlyCollection(IEnumerable<T> items)
		{
			_internalList = new PapyrusList<T>(items);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return List.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(T item)
		{
			throw new InvalidOperationException("Add called on ReadOnly collection");
		}

		public void Clear()
		{
			throw new InvalidOperationException("Clear called on ReadOnly collection");
		}

		public bool Contains(T item)
		{
			return List.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			List.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			throw new InvalidOperationException("Remove called on ReadOnly collection");
		}

		public void CopyTo(Array array, int index)
		{
			List.CopyTo((T[])array, index);
		}

		public int Count { get { return List.Count; } }
		public object SyncRoot { get { return null; } }

		public bool IsSynchronized { get { return false; } }

		public bool IsReadOnly { get { return true; } }

		public bool Equals(ReadOnlyCollection<T> other)
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

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			return obj is ReadOnlyCollection<T> && Equals((ReadOnlyCollection<T>) obj);
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}

		public static bool operator ==(ReadOnlyCollection<T> left, ReadOnlyCollection<T> right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ReadOnlyCollection<T> left, ReadOnlyCollection<T> right)
		{
			return !left.Equals(right);
		}


		public int IndexOf(T item)
		{
			return List.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			throw new InvalidOperationException("Insert called on ReadOnly collection");
		}

		public void RemoveAt(int index)
		{
			throw new InvalidOperationException("RemoveAt called on ReadOnly collection");
		}

		public T this[int index]
		{
			get { return List[index]; }
			set
			{
				throw new InvalidOperationException("ReadOnlyCollection cannot be modified");
			}
		}
	}

}
