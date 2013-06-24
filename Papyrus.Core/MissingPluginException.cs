/*
 * Copyright © 2013 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
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