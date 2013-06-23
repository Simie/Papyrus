using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Papyrus.Core.Util
{

	public static class RecordReflectionUtil
	{


		/// <summary>
		/// Get a list of serializable properties in a record
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		internal static List<PropertyInfo> GetProperties<T>() where T : Record
		{
			return GetProperties(typeof (T));
		} 

		internal static List<PropertyInfo> GetProperties(Type t)
		{

			var props = t.GetProperties().Where(p => !p.HasAttribute<Newtonsoft.Json.JsonIgnoreAttribute>());

			return props.ToList();

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

	}

}
