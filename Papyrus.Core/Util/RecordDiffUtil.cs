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

namespace Papyrus.Core.Util
{

	public struct PropertyDiff
	{

		public PropertyInfo Property;

		public object OldValue;

		public object NewValue;

	}

	public static class RecordDiffUtil
	{

		/// <summary>
		/// Produce a list of differences between two records of the same type
		/// </summary>
		/// <typeparam name="T">Record Type</typeparam>
		/// <param name="r1">Original Record</param>
		/// <param name="r2">New Record</param>
		/// <returns>A list of <c>PropertyDiff</c></returns>
		public static IList<PropertyDiff> Diff<T>(T r1, T r2) where T : Record
		{
			
			if(r1 == r2)
				throw new InvalidOperationException("Attempted to diff a record with itself");

			if(r1.GetType() != r2.GetType())
				throw new InvalidOperationException("Attempted to diff records of different type");

			var properties = RecordReflectionUtil.GetProperties<T>();

			var differences = new List<PropertyDiff>();

			foreach (var property in properties) {

				PropertyDiff? result;

				if(!DiffProperty(property, r1, r2, out result) || !result.HasValue)
					continue;
				
				differences.Add(result.Value);

			}

			return differences;

		} 

		private static bool DiffProperty(PropertyInfo property, object r1, object r2, out PropertyDiff? diff)
		{

			object v1 = property.GetValue(r1, null);
			object v2 = property.GetValue(r2, null);

			if (v1 != null && v1.Equals(v2)) {

				diff = null;
				return false;

			}

			diff = new PropertyDiff {OldValue = v1, NewValue = v2, Property = property};

			return true;

		}

	}

}
