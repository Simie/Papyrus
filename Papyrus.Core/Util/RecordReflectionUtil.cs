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

		/*public static T Resolve<T>(this RecordDatabase db, RecordRef<T> recordRef) where T : Record
		{
			
		}*/

	}

}
