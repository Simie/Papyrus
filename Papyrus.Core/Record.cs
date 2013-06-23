using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Papyrus.Core
{

	public class Record : Freezable
	{

		// Store the key in an internal field to prevent user setting it from SetProperty (IFreezable)
		[JsonIgnore]
		internal RecordKey InternalKey = RecordKey.Identity;

		/// <summary>
		/// Key that can be used to reference this record again later (for example, when loading from a save game)
		/// </summary>
		[JsonIgnore]
		public RecordKey Key { get { return InternalKey; } }

		/// <summary>
		/// A friendly name to appear in the editor (default to RecordKey)
		/// </summary>
		public string EditorID { get; private set; }

		/// <summary>
		/// Comparer which only uses the Key field of a record for comparison.
		/// </summary>
		public static IEqualityComparer<Record> KeyComparer
		{
			get { return KeyComparerInstance; }
		}

		// TODO: ProperyComparer (use reflection to get serializable properties, compare only using those properties)

		private sealed class KeyEqualityComparer : IEqualityComparer<Record>
		{

			public bool Equals(Record x, Record y)
			{
				if (ReferenceEquals(x, y))
					return true;
				if (ReferenceEquals(x, null))
					return false;
				if (ReferenceEquals(y, null))
					return false;
				if (x.GetType() != y.GetType())
					return false;
				return x.InternalKey.Equals(y.InternalKey);
			}

			public int GetHashCode(Record obj)
			{
				return obj.InternalKey.GetHashCode();
			}

		}

		private static readonly IEqualityComparer<Record> KeyComparerInstance = new KeyEqualityComparer();

	}

}
