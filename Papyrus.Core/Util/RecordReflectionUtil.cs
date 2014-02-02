/*
 * Copyright © 2014 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Papyrus.Core.Util
{

	/// <summary>
	/// Utilities related to reflection
	/// </summary>
	public static class RecordReflectionUtil
	{

		/// <summary>
		/// Get a list of serializable properties in record type
		/// </summary>
		/// <param name="includeReadOnly"></param>
		/// <typeparam name="T">Include properties which cannot be written to (used in editor for comments)</typeparam>
		/// <returns></returns>
		public static List<PropertyInfo> GetProperties<T>(bool includeReadOnly = false) where T : Record
		{
			return GetProperties(typeof (T), includeReadOnly);
		}

		/// <summary>
		/// Get list of serializable properties in record type
		/// </summary>
		/// <param name="t"></param>
		/// <param name="includeReadOnly">Include properties which cannot be written to (used in editor for comments)</param>
		/// <returns></returns>
		public static List<PropertyInfo> GetProperties(Type t, bool includeReadOnly = false)
		{

			// Check base class first, since calling GetProperties() on child class doesn't return setter
			var properties = t.BaseType != null ? GetProperties(t.BaseType) : new List<PropertyInfo>();

			properties.AddRange(
				t.GetProperties()
					.Where(p => (includeReadOnly || p.CanWrite) && !p.HasAttribute<JsonIgnoreAttribute>())
					// Ensure unique properties only
					.Except(properties, PropertyInfoNameEqualityComparer.Instance));

			return properties;

		}

		/// <summary>
		/// Get a list of RecordRef properties in a record type.
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		internal static List<PropertyInfo> GetReferenceProperties(Type t)
		{

			return
				GetProperties(t)
					.Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof (RecordRef<>)).ToList();

		}
		
		/// <summary>
		/// Get a list of RecordRef properties in a record type.
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		internal static List<PropertyInfo> GetReferenceCollectionProperties(Type t)
		{

			return
				GetProperties(t)
					.Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof (RecordRefCollection<>)).ToList();

		} 

		/// <summary>
		/// Clone all record properties and key to a new record
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		internal static Record Clone(this Record record)
		{

			var type = record.GetType();
			var clone = (Record)Activator.CreateInstance(type);

			Populate(record, clone);

			return clone;

		}

		/// <summary>
		/// Populate dest with the property values from source
		/// </summary>
		/// <param name="source">Copy source</param>
		/// <param name="dest">Copy destination</param>
		/// <param name="autoUnfreeze">Automatically set IsFrozen to allow the populate to succeed</param>
		internal static void Populate(Record source, Record dest, bool autoUnfreeze = false)
		{

			if (source.GetType() != dest.GetType()) 
				throw new ArgumentException("source and dest type do not match");

			var type = source.GetType();

			var origFreeze = dest.IsFrozen;

			if (autoUnfreeze)
				dest.IsFrozen = false;

			// Populate dest record with json properties
			RecordSerializer.FromJson(RecordSerializer.ToJson(source), type, dest);
			dest.InternalKey = source.InternalKey;

			dest.IsFrozen = origFreeze;

		}

		/// <summary>
		/// Search all loaded assemblies for record types
		/// </summary>
		/// <returns></returns>
		public static List<Type> GetRecordTypes()
		{

			return (
				       from assembly in AppDomain.CurrentDomain.GetAssemblies()
				       from type in assembly.GetTypes()
				       where type.IsSubclassOf(typeof (Record))
				       select type
			       ).ToList();

		}

		private class PropertyInfoNameEqualityComparer : EqualityComparer<PropertyInfo>
		{

			public static readonly PropertyInfoNameEqualityComparer Instance = new PropertyInfoNameEqualityComparer();

			public override int GetHashCode(PropertyInfo obj)
			{
				return obj == null ? 0 : obj.Name.GetHashCode();
			}

			public override bool Equals(PropertyInfo x, PropertyInfo y)
			{
				return x.Name.Equals(y.Name);
			}

		}


	}

}
