/*
 * Copyright © 2014 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Papyrus.Core.Util
{

	/// <summary>
	/// Sourced from http://stackoverflow.com/a/2441448/147003 under CC BY-SA 3.0
	/// </summary>
	internal static class EnumerableExtensions
	{
		public static bool IsEquivalentTo<T>(this IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer = null)
		{

			if (comparer == null)
				comparer = EqualityComparer<T>.Default;

			var secondList = second.ToList();
			
			foreach (var item in first) {

				var index = secondList.FindIndex(p => comparer.Equals(item, p));

				if (index < 0) {
					return false;
				}

				secondList.RemoveAt(index);

			}

			return secondList.Count == 0;

		}

		public static bool IsSubsetOf(this IEnumerable first, IEnumerable second)
		{
			var secondList = second.Cast<object>().ToList();
			foreach (var item in first) {
				var index = secondList.FindIndex(item.Equals);
				if (index < 0) {
					return false;
				}
				secondList.RemoveAt(index);
			}
			return true;
		}

		/// <summary>
		/// Topological sort
		/// </summary>
		/// <remarks>http://stackoverflow.com/a/11027096/147003</remarks>
		public static IEnumerable<T> TSort<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> dependencies)
		{
			var sorted = new List<T>();
			var visited = new HashSet<T>();

			foreach (var item in source)
				Visit(item, visited, sorted, dependencies);

			return sorted;
		}

		private static void Visit<T>(T item, HashSet<T> visited, List<T> sorted, Func<T, IEnumerable<T>> dependencies)
		{
			if (!visited.Contains(item)) {
				visited.Add(item);

				foreach (var dep in dependencies(item))
					Visit(dep, visited, sorted, dependencies);

				sorted.Add(item);
			} else if (!sorted.Contains(item)) { throw new Exception("Invalid dependency cycle!"); }
		}

	}

}
