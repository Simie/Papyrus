using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papyrus.Core.Util
{

	internal static class RecordUtils
	{

		public static ICollection<IRecordRef> GetReferences(Record rec)
		{

			return RecordReflectionUtil.GetReferenceProperties(rec.GetType()).Select(p => (IRecordRef) p.GetValue(rec, null)).ToArray();

		} 

	}

}
