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
