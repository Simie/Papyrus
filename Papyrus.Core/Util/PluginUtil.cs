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
	/// Plugin related utilities
	/// </summary>
	public static class PluginUtil
	{

		/// <summary>
		/// Sort a plugin list, ensuring that parent plugins load before child plugins
		/// </summary>
		/// <param name="plugins"></param>
		/// <returns></returns>
		public static IList<Plugin> SortPluginList(IList<Plugin> plugins)
		{
			// Sort plugins by dependencies
			return plugins.TSort(
				// Convert list of plugin names to plugin objects
				plugin => plugin.Parents.Select(p => plugins.FirstOrDefault(q => q.Name == p))
			).ToList();

		} 
		
		/// <summary>
		/// Sort list of T using the same plugin sorting algorithm as non-generic version, but allow for a selector to provide the plugin name.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="selector"></param>
		/// <returns></returns>
		public static IList<T> SortPluginList<T>(IList<T> list, Func<T, Plugin> selector)
		{
			// Sort plugins by dependencies
			return list.TSort(
				// Convert list of plugin names to plugin objects
				plugin => selector(plugin).Parents.Select(p => list.FirstOrDefault(q => selector(q).Name == p))
			).ToList();
		} 

	}
}
