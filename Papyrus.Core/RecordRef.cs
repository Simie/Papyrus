/*
 * Copyright © 2013 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papyrus.Core
{

	public interface IRecordRef
	{

		/// <summary>
		/// Record Key
		/// </summary>
		RecordKey Key { get; }

		/// <summary>
		/// Reference Type
		/// </summary>
		Type Type { get; }
		
		/// <summary>
		/// The actual record value type. Can differ from Reference Type if the reference is to a subclass of T
		/// </summary>
		Type ValueType { get; }

	}
		
	public struct RecordRef<T> : IEquatable<IRecordRef>, IRecordRef where T : Record
	{

		/// <summary>
		/// Record Key
		/// </summary>
		public RecordKey Key
		{
			get { return _key; }
			private set { _key = value; }
		}

		/// <summary>
		/// Reference Type
		/// </summary>
		public Type Type { get { return typeof (T); } }

		/// <summary>
		/// The actual record value type. Can differ from Reference Type if the reference is to a subclass of T
		/// </summary>
		public Type ValueType
		{
			get
			{
				if (_valueType == null)
					return Type;
				return _valueType;
			}
		}

		private Type _valueType;

		// Use field so Unity3D can serialize correctly
		private RecordKey _key;

		/// <summary>
		/// Create a new RecordRef object, with the specified key. Optionally pass a type for polymorphic behaviour
		/// </summary>
		/// <param name="key">Record key</param>
		/// <param name="type">Record type. Must be subclass of T</param>
		public RecordRef(RecordKey key, Type type = null) : this()
		{
			Key = key;

			if (type == null || type == Type) {

				_valueType = Type;

			} else {

				if (!type.IsSubclassOf(Type))
					throw new ArgumentException("type is not subclass of reference type", "type");

				_valueType = type;

			}

		}

		/// <summary>
		/// Create a record reference from a given record
		/// </summary>
		/// <param name="record"></param>
		public RecordRef(T record) : this(record.Key, record.GetType()) {}

		/// <summary>
		/// Create a RecordRef from the string form
		/// </summary>
		/// <param name="str"></param>
		internal RecordRef(string str) : this()
		{

			try {

				// Check for polymorphic reference
				if (str.Contains(',')) {

					var split = str.Split(',');
					Key = RecordKey.FromString(split[0]);
					_valueType = Util.ReflectionUtil.ResolveRecordType(split[1]);

				} else {

					Key = RecordKey.FromString(str);

				}

			} catch (Exception e) {
				throw new ArgumentException("String was not in expected format", "str", e);
			}

		}

		public override string ToString()
		{

			var key = Key.ToString();

			if (ValueType != Type) {
				key += ", " + (ValueType.FullName);
			}

			return key;

		}

		/// <summary>
		/// Check for equality with another record reference object
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(IRecordRef other)
		{
			return Key.Equals(other.Key) && ValueType == other.ValueType;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			return obj is IRecordRef && Equals((IRecordRef) obj);
		}

		public override int GetHashCode()
		{
			return Key.GetHashCode();
		}

		public static bool operator ==(RecordRef<T> left, RecordRef<T> right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(RecordRef<T> left, RecordRef<T> right)
		{
			return !left.Equals(right);
		}

	}

}
