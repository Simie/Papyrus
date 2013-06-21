using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papyrus.Core
{

	public class Record : Freezable
	{

		/// <summary>
		/// A friendly name to appear in the editor (default to RecordKey)
		/// </summary>
		public string EditorID { get; private set; }

	}

}
