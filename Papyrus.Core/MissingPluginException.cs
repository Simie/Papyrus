using System;
using System.Runtime.Serialization;

namespace Papyrus.Core
{
	[Serializable]
	public class MissingPluginException : Exception
	{

		public string Plugin { get; private set; }

		public MissingPluginException() {}

		public MissingPluginException(string message, string plugin) : base(message)
		{
			Plugin = plugin;
		}

		public MissingPluginException(string message, string plugin, Exception inner) : base(message, inner)
		{
			Plugin = plugin;
		}

		protected MissingPluginException(
			SerializationInfo info,
			StreamingContext context) : base(info, context) {}

	}
}