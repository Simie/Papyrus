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

		RecordKey Key { get; }
		Type Type { get; }

	}
		
	public struct RecordRef<T> : IEquatable<RecordRef<T>>, IRecordRef where T : Record
	{

		public RecordKey Key { get; private set; }

		public Type Type { get { return typeof (T); } }

		public RecordRef(RecordKey key) : this()
		{
			Key = key;
		} 

		public bool Equals(RecordRef<T> other)
		{
			return Key.Equals(other.Key);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			return obj is RecordRef<T> && Equals((RecordRef<T>) obj);
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
