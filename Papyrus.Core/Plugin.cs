using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papyrus.Core
{

	/// <summary>
	/// A collection of record lists, with add/remove operations
	/// </summary>
	public class Plugin
	{

		public string Name { get; private set; }

		internal RecordCollection Records { get; private set; }

		Plugin()
		{
			
			
		}

	}

}
