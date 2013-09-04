/*
 * Copyright © 2013 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
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
using Papyrus.Core.Util;

namespace Papyrus.Core
{

	/// <summary>
	/// A struct-based Read Only collection for Papyrus Records
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public struct ReadOnlyCollection<T> : ICollection<T>, ICollection, IEquatable<ReadOnlyCollection<T>>
	{

		private List<T> _internalList;

		/// <summary>
		/// Access the collection
		/// </summary>
		private IList<T> List {

			get {

				if(_internalList == null)
					return new T[0];
				return _internalList.AsReadOnly();

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
			throw new NotSupportedException();
		}

		public void Clear()
		{
			throw new NotSupportedException();
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
			throw new NotSupportedException();
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
			return other.List.IsEquivalentTo(List);
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

	}

}
