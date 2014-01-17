/*
 * Copyright © 2013 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;

namespace Papyrus.Core
{

	/// <summary>
	/// An absolute reference to a record. Used for database lookup
	/// </summary>
	[JsonConverter(typeof(Util.JsonConverters.RecordKeyConverter))]
	public struct RecordKey : IEquatable<RecordKey>
	{

		/// <summary>
		/// A blank record key
		/// </summary>
		public static readonly RecordKey Identity = new RecordKey(-1);

		public const char Seperator = '/';

		/// <summary>
		/// Name of plugin the key is referencing
		/// </summary>
		public string Plugin
		{
			get { return _plugin; }
			internal set { _plugin = value; }
		}

		/// <summary>
		/// Record index
		/// </summary>
		public int Index
		{
			get { return _index; }
			internal set { _index = value; }
		}

		private string _plugin;
		private int _index;

		public RecordKey(int index, string plugin = null) : this()
		{
			Index = index;
			Plugin = plugin;
		}

		public RecordKey(string str)
		{
			this = FromString(str);
		}

		public bool Equals(RecordKey other)
		{
			return string.Equals(Plugin, other.Plugin) && Index == other.Index;
		}

		/// <summary>
		/// Create a RecordKey from string representation.
		/// </summary>
		/// <param name="str">Key string. Sample formats: "[PluginName]/[RecordIndex]" or "[RecordIndex]"</param>
		/// <returns></returns>
		public static RecordKey FromString(string str)
		{

			try {

				var split = str.Split(Seperator);

				int index = 0;

				return new RecordKey {

					Plugin = split.Length == 2 ? split[index++] : null,
					Index = int.Parse(split[index], NumberStyles.AllowHexSpecifier)

				};

			} catch (Exception e) {

				throw new FormatException("String was not in expected format", e);

			}

		}

		public override string ToString()
		{
			if(Plugin != null)
				return string.Format("{0}{2}{1:X6}", Plugin, Index, Seperator);
			return string.Format("{0:X6}", Index);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			return obj is RecordKey && Equals((RecordKey)obj);
		}

		public override int GetHashCode()
		{
			unchecked {
				return ((Plugin != null ? Plugin.GetHashCode() : 0) * 397) ^ Index;
			}
		}

		public static bool operator ==(RecordKey left, RecordKey right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(RecordKey left, RecordKey right)
		{
			return !left.Equals(right);
		}

		/// <summary>
		/// A simple concrete equality comparer to fix AOT errors when with Unity
		/// </summary>
		private sealed class EqualityComparer : IEqualityComparer<RecordKey>
		{

			public bool Equals(RecordKey x, RecordKey y)
			{
				return x.Equals(y);
			}

			public int GetHashCode(RecordKey obj)
			{
				unchecked {
					return obj.GetHashCode();
				}
			}

		}

		private static readonly IEqualityComparer<RecordKey> ComparerInstance = new EqualityComparer();

		public static IEqualityComparer<RecordKey> Comparer
		{
			get { return ComparerInstance; }
		}

	}

}
