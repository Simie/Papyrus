/*
 * Copyright © 2014 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Papyrus.Core
{

	/// <summary>
	/// Same as <c>System.Collections.Generic.List</c> but with appropriate JsonArray attribute to allow serializing
	/// subclasses of the generic type parameter. A bit of a hack, but it works until JsonProperty attributes are used properly
	/// during Record serialization.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[JsonArray(ItemTypeNameHandling = TypeNameHandling.Auto)]
	public class PapyrusList<T> : List<T>
	{

		/// <summary>
		/// Construct empty PapyrusList instance
		/// </summary>
		public PapyrusList() { }

		/// <summary>
		/// Construct new PapyrusList with the contents of collection
		/// </summary>
		/// <param name="collection"></param>
		public PapyrusList(IEnumerable<T> collection) : base(collection) {}

		/// <summary>
		/// Construct new PapyrusList with capacity
		/// </summary>
		/// <param name="capacity"></param>
		public PapyrusList(int capacity) : base(capacity) {}

	}

}
