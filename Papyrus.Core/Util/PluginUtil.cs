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
using System.Text;

namespace Papyrus.Core.Util
{
	public static class PluginUtil
	{

		public static IList<Plugin> SortPluginList(IList<Plugin> plugins)
		{
			// Sort plugins by dependencies
			return plugins.TSort(
				// Convert list of plugin names to plugin objects
				plugin => plugin.Parents.Select(p => plugins.FirstOrDefault(q => q.Name == p))
			).ToList();

		} 
		
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
