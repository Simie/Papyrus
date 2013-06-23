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

		internal Plugin(string name)
		{

			Name = name;

		}

		/// <summary>
		/// Get a key for the next record
		/// </summary>
		/// <param name="recordType"></param>
		/// <returns></returns>
		internal RecordKey NextKey(Type recordType)
		{

			var records = Records.GetRecords(recordType);
			
			var nextIndex = records.Max(p => p.InternalKey.Index)+1;

			return new RecordKey(nextIndex, Name);

		}

	}

}
