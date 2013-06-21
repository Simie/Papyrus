using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papyrus.Core
{

	public struct RecordRef<T> where T : Record
	{

		public RecordKey Key { get; internal set; }

		public T Value { get; internal set; }

	}

}
