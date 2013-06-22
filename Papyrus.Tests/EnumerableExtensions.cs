using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Papyrus.Tests
{

	/// <summary>
	/// Sourced from http://stackoverflow.com/a/2441448/147003 under CC BY-SA 3.0
	/// </summary>
	public static class EnumerableExtensions
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
	}

}
