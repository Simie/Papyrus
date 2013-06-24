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
