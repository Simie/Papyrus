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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Papyrus.Core.Util.JsonConverters;

namespace Papyrus.Core.Util
{
	internal static class Serialization
	{

		public static Newtonsoft.Json.JsonSerializerSettings GetJsonSettings()
		{

			var settings = new JsonSerializerSettings();

			// Default to indented, for easy user-editing
			settings.Formatting = Formatting.Indented;

			settings.Converters.Add(new StringEnumConverter());
			settings.Converters.Add(new RecordKeyConverter());
			settings.Converters.Add(new RecordRefConverter());
			settings.Converters.Add(new RecordCollectionConverter());
			settings.Converters.Add(new RecordConverter());
			settings.Converters.Add(new RecordRefCollectionConverter());
			settings.Converters.Add(new ReadOnlyCollectionConverter());

			return settings;

		}

		/// <summary>
		/// Get a json serializer tunes for papyrus
		/// </summary>
		/// <returns></returns>
		public static Newtonsoft.Json.JsonSerializer GetJsonSerializer()
		{

			return JsonSerializer.Create(GetJsonSettings());

		}

	}
}
