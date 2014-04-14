/*
 * Copyright © 2014 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;
using System.Runtime.Serialization;

namespace Papyrus.Core
{

	/// <summary>
	/// Exception thrown when a plugin cannot be loaded due to missing parent
	/// </summary>
	[Serializable]
	public class MissingPluginException : Exception
	{

		/// <summary>
		/// Missing plugin name
		/// </summary>
		public string Plugin { get; private set; }

		/// <summary>
		/// Exception message, with plugin name appended
		/// </summary>
		public override string Message
		{
			get { return string.Format("{0} ({1})", base.Message, Plugin); }
		}

		/// <summary>
		/// Create MissingPluginException object
		/// </summary>
		public MissingPluginException() {}

		/// <summary>
		/// Create MissingPluginException object with message, and plugin string
		/// </summary>
		/// <param name="message"></param>
		/// <param name="plugin"></param>
		public MissingPluginException(string message, string plugin) : base(message)
		{
			Plugin = plugin;
		}

		/// <summary>
		/// Create MissingPluginException object with message, plugin string and inner exception
		/// </summary>
		/// <param name="message"></param>
		/// <param name="plugin"></param>
		/// <param name="inner"></param>
		public MissingPluginException(string message, string plugin, Exception inner) : base(message, inner)
		{
			Plugin = plugin;
		}

	}
}