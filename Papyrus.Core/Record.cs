﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

		/// <summary>
		/// Comparer which uses reflection to compare papyrus properties for equality.
		/// </summary>
		public static IEqualityComparer<Record> PropertyComparer
		{
			get { return PropertyComparerInstance; }
		}

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

		private sealed class PropertyEqualityComparer : IEqualityComparer<Record>
		{

			public bool Equals(Record x, Record y)
			{

				if (x.GetType() != y.GetType())
					return false;

				var properties = Util.RecordReflectionUtil.GetProperties(x.GetType());

				return properties.All((prop) => CheckProperty(x, y, prop));

			}

			private bool CheckProperty(Record x, Record y, PropertyInfo prop)
			{

				var vX = prop.GetValue(x, null);
				var vY = prop.GetValue(y, null);

				if (vX == null && vY == null)
					return true;

				if (vX != null && vY == null)
					return false;

				if (vX == null)
					return false;

				return vX.Equals(vY);

			}

			public int GetHashCode(Record obj)
			{
				throw new NotImplementedException();
			}

		}

		private static readonly IEqualityComparer<Record> PropertyComparerInstance = new PropertyEqualityComparer();

	}

}
