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

		private List<T> _internalList;

		/// <summary>
		/// Access the collection
		/// </summary>
		private IList<T> List {

			get {

				if (_internalList == null) _internalList = new List<T>(0);
				return _internalList;

			}

		}

		/// <summary>
		/// Create a new ReadOnlyCollection with the specified items
		/// </summary>
		/// <param name="items"></param>
		public ReadOnlyCollection(IEnumerable<T> items)
		{
			_internalList = new List<T>(items);
		}

		/// <summary>
		/// Enumerate collection
		/// </summary>
		/// <returns></returns>
		public IEnumerator<T> GetEnumerator()
		{
			return List.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Not supported
		/// </summary>
		public void Add(T item)
		{
			throw new InvalidOperationException("Add called on ReadOnly collection");
		}

		/// <summary>
		/// Not supported
		/// </summary>
		public void Clear()
		{
			throw new InvalidOperationException("Clear called on ReadOnly collection");
		}

		/// <summary>
		/// Not supported
		/// </summary>
		public bool Contains(T item)
		{
			return List.Contains(item);
		}

		/// <summary>
		/// Copy collection to array
		/// </summary>
		public void CopyTo(T[] array, int arrayIndex)
		{
			List.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Not supported
		/// </summary>
		public bool Remove(T item)
		{
			throw new InvalidOperationException("Remove called on ReadOnly collection");
		}

		/// <summary>
		/// Copy collection to array
		/// </summary>
		public void CopyTo(Array array, int index)
		{
			List.CopyTo((T[])array, index);
		}

		/// <summary>
		/// Number of items in collection
		/// </summary>
		public int Count { get { return List.Count; } }

		/// <summary>
		/// Not supported
		/// </summary>
		public object SyncRoot { get { throw new NotSupportedException(); } }

		/// <summary>
		/// Not supported
		/// </summary>
		public bool IsSynchronized { get { return false; } }

		/// <summary>
		/// Is this collection read only (yes)
		/// </summary>
		public bool IsReadOnly { get { return true; } }

		/// <summary>
		/// Does this collection equal another collection
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(ReadOnlyCollection<T> other)
		{

			if (other._internalList == null && _internalList == null)
				return true;

			if (other._internalList == null || _internalList == null)
				return false;

			if (_internalList.Count != other._internalList.Count)
				return false;

			for (var i = 0; i < _internalList.Count; i++) {
				if (!EqualityComparer<T>.Default.Equals(_internalList[i], other._internalList[i]))
					return false;
			}

			return true;

		}

		/// <summary>
		/// Does this object equal another object
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			return obj is ReadOnlyCollection<T> && Equals((ReadOnlyCollection<T>) obj);
		}

		/// <summary>
		/// Not supported
		/// </summary>
		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Check two collections for equality
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator ==(ReadOnlyCollection<T> left, ReadOnlyCollection<T> right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Check two collections for inequality
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator !=(ReadOnlyCollection<T> left, ReadOnlyCollection<T> right)
		{
			return !left.Equals(right);
		}

		/// <summary>
		/// Return index of item in list
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public int IndexOf(T item)
		{
			return List.IndexOf(item);
		}

		/// <summary>
		/// Not supported
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		public void Insert(int index, T item)
		{
			throw new InvalidOperationException("Insert called on ReadOnly collection");
		}

		/// <summary>
		/// Not supported
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt(int index)
		{
			throw new InvalidOperationException("RemoveAt called on ReadOnly collection");
		}

		/// <summary>
		/// Return element at index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
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
