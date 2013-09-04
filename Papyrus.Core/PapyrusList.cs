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
	public class PapyrusList<T> : List<T> { }

}
