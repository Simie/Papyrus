﻿/*
 * Copyright © 2014 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
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

	internal struct PropertyDiff
	{

		public PropertyInfo Property;

		public object OldValue;

		public object NewValue;

	}

	internal static class RecordDiffUtil
	{

		/// <summary>
		/// Produce a list of differences between two records of the same type
		/// </summary>
		/// <param name="oldRecord">Original Record</param>
		/// <param name="newRecord">New Record</param>
		/// <returns>A list of <c>PropertyDiff</c></returns>
		public static IList<PropertyDiff> Diff(Record oldRecord, Record newRecord)
		{

			if (oldRecord == newRecord)
				throw new InvalidOperationException("Attempted to diff a record with itself");

			if (oldRecord.GetType() != newRecord.GetType())
				throw new InvalidOperationException("Attempted to diff records of different type");

			var properties = RecordReflectionUtil.GetProperties(oldRecord.GetType());

			var differences = new List<PropertyDiff>();

			foreach (var property in properties) {

				PropertyDiff? result;

				if (!DiffProperty(property, oldRecord, newRecord, out result) || !result.HasValue)
					continue;

				differences.Add(result.Value);

			}

			return differences;

		} 

		private static bool DiffProperty(PropertyInfo property, object r1, object r2, out PropertyDiff? diff)
		{

			object v1 = property.GetValue(r1, null);
			object v2 = property.GetValue(r2, null);


			if ((v1 == null && v2 == null) || (v1 != null && v1.Equals(v2)) || (v2 != null && v2.Equals(v1))) {

				diff = null;
				return false;

			}

			diff = new PropertyDiff {OldValue = v1, NewValue = v2, Property = property};

			return true;

		}

	}

}
