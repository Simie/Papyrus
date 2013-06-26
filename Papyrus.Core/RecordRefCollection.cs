using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;

namespace Papyrus.Core
{

	public interface IRecordRefCollection : IEnumerable
	{

		Type RecordType { get; }

		IList<IRecordRef> References { get; } 

	}

	[JsonObject]
	public struct RecordRefCollection<T> : IRecordRefCollection, IEnumerable<RecordRef<T>> where T : Record
	{

		private readonly List<RecordRef<T>> _internalList;
			
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

	}

}
