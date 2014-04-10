/*
 * Copyright © 2014 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papyrus.Core.Util
{

	/// <summary>
	/// Collection of general utilities for Records
	/// </summary>
	public static class RecordUtils
	{

		/// <summary>
		/// Return a string representation of the record
		/// </summary>
		/// <param name="rec"></param>
		/// <returns></returns>
		public static string GetDebugString(Record rec)
		{

			var props = RecordReflectionUtil.GetProperties(rec.GetType());
			var o = new StringBuilder();

			var length = props.Max(p => p.Name.Length);

			foreach (var propertyInfo in props) {

				o.AppendFormat("{0, "+length+"}: {1}", propertyInfo.Name, propertyInfo.GetValue(rec, null));
				o.AppendLine();

			}

			return o.ToString();

		}

		internal static ICollection<IRecordRef> GetReferences(Record rec)
		{

			var basicReferences = RecordReflectionUtil.GetReferenceProperties(rec.GetType()).Select(p => (IRecordRef) p.GetValue(rec, null));

			var collectionReferences =
				RecordReflectionUtil.GetReferenceCollectionProperties(rec.GetType())
				                    .SelectMany(p => ((IRecordRefCollection) p.GetValue(rec, null)).References);

			return basicReferences.Concat(collectionReferences).ToArray();

		} 

	}

}
