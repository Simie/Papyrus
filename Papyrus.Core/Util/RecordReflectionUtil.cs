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
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Papyrus.Core.Util
{

	public static class RecordReflectionUtil
	{


		/// <summary>
		/// Get a list of serializable properties in a record
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static List<PropertyInfo> GetProperties<T>() where T : Record
		{
			return GetProperties(typeof (T));
		} 

		public static List<PropertyInfo> GetProperties(Type t)
		{

			var props = t.GetProperties().Where(p => !p.HasAttribute<JsonIgnoreAttribute>());

			return props.ToList();

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
		internal static void Populate(Record source, Record dest)
		{

			if (source.GetType() != dest.GetType()) 
				throw new ArgumentException("source and dest type do not match");

			var type = source.GetType();

			dest.InternalKey = source.InternalKey;

			var properties = GetProperties(type);

			foreach (var property in properties) {

				dest.SetProperty(property.Name, property.GetValue(source, null));

			}

		}


		/*public static T Resolve<T>(this RecordDatabase db, RecordRef<T> recordRef) where T : Record
		{
			
		}*/

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

	}

}
