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
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Papyrus.Core.Util.JsonConverters;

namespace Papyrus.Core.Util
{
	internal static class Serialization
	{

		private static JsonSerializerSettings _settingsInstance;
		private static JsonSerializer _serializerInstance;

		public static Newtonsoft.Json.JsonSerializerSettings GetJsonSettings()
		{

			if (_settingsInstance != null)
				return _settingsInstance;

			_settingsInstance = new JsonSerializerSettings();

			// Default to indented, for easy user-editing
			_settingsInstance.Formatting = Formatting.Indented;

			_settingsInstance.Converters.Add(new StringEnumConverter());

			return _settingsInstance;

		}

		/// <summary>
		/// Get a json serializer tunes for papyrus
		/// </summary>
		/// <returns></returns>
		public static Newtonsoft.Json.JsonSerializer GetJsonSerializer()
		{

			return _serializerInstance ?? JsonSerializer.Create(GetJsonSettings());

		}

		/// <summary>
		/// Strip comments from json file
		/// </summary>
		/// <param name="json"></param>
		/// <returns></returns>
		public static string StripComments(string json)
		{
			return Regex.Replace(json, @"/\*(.*?)\*/", "");
		}

	}
}
