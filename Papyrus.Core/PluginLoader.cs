/*
 * Copyright © 2014 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Papyrus.Core.Util;

namespace Papyrus.Core
{

	/// <summary>
	/// Helper utils for loading plugins
	/// </summary>
	public static class PluginLoader
	{

		/// <summary>
		/// Scan a directory and load all plugins within
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static IList<Plugin> ScanDirectory(string path)
		{

			if(!Directory.Exists(path))
				throw new DirectoryNotFoundException("Directory not found " + path);

			var directory = new DirectoryInfo(path);

			var pluginFiles = directory.GetFiles("*." + Plugin.Extension);

			var plugins = new List<Plugin>(pluginFiles.Length);

			foreach (var pluginFile in pluginFiles) {
				
				try {
					plugins.Add(LoadPluginFile(pluginFile.FullName));
				} catch(Exception e) {
					Console.WriteLine("Error Reading Plugin: " + pluginFile.FullName);
					Console.WriteLine(e.ToString());
				}

			}

			return plugins;

		}

		/// <summary>
		/// Load a plugin from a JSON string
		/// </summary>
		/// <param name="pluginJson"></param>
		/// <returns></returns>
		public static Plugin LoadPluginString(string pluginJson)
		{
			return PluginSerializer.FromJson(pluginJson, false);
		}

		/// <summary>
		/// Load a plugin from the file at filePath
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static Plugin LoadPluginFile(string filePath)
		{
			
			if(!File.Exists(filePath))
				throw new FileNotFoundException("File not found", filePath);

			var jsonText = File.ReadAllText(filePath, Encoding.UTF8);

			return LoadPluginString(jsonText);

		}

		/// <summary>
		/// Save plugin to directory
		/// </summary>
		/// <param name="plugin"></param>
		/// <param name="directory"></param>
		/// <param name="parentCollection">Create partial records from this collection</param>
		/// <returns></returns>
		internal static bool SavePlugin(Plugin plugin, string directory, RecordCollection parentCollection)
		{

			if (!Directory.Exists(directory))
				throw new DirectoryNotFoundException("Directory not found " + directory);

			var savePath = Path.Combine(directory, plugin.Name + "." + Plugin.Extension);

			// Ensure the latest dependencies are known
			plugin.RefreshParents();

			var pluginJson = PluginSerializer.ToJson(plugin, parentCollection);

			File.WriteAllText(savePath, pluginJson, Encoding.UTF8);

			return true;

		}

		/// <summary>
		/// Save plugin to stream
		/// </summary>
		/// <param name="plugin"></param>
		/// <param name="writer"></param>
		/// <param name="parentCollection">Create partial records from this collection</param>
		/// <returns></returns>
		internal static bool SavePlugin(Plugin plugin, TextWriter writer, RecordCollection parentCollection)
		{

			// Ensure the latest dependencies are known
			plugin.RefreshParents();

			var pluginJson = PluginSerializer.ToJson(plugin, parentCollection);

			writer.Write(pluginJson);

			return true;

		}

	}

}
