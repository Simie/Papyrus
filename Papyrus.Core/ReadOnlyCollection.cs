using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papyrus.Core
{

	public class ReadOnlyCollection<T> : ICollection<T>
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

		public int Count { get { return List.Count; } }
		public bool IsReadOnly { get { return true; } }

	}

}
