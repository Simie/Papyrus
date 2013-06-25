/*
 * Copyright © 2013 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
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

			var basicReferences = RecordReflectionUtil.GetReferenceProperties(rec.GetType()).Select(p => (IRecordRef) p.GetValue(rec, null));

			var collectionReferences =
				RecordReflectionUtil.GetReferenceCollectionProperties(rec.GetType())
				                    .SelectMany(p => ((IRecordRefCollection) p.GetValue(rec, null)).References);

			return basicReferences.Concat(collectionReferences).ToArray();

		} 

	}

}
