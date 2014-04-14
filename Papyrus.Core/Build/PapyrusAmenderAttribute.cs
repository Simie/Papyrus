using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Afterthought;

namespace Papyrus.Core.Build
{
	/// <summary>
	/// Add this attribute to an assembly to trigger papyrus post-processing when Afterthought is triggered
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	public class PapyrusAmenderAttribute :  Attribute, IAmendmentAttribute
	{

		/// <summary>
		/// Get amenders for type
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public IEnumerable<ITypeAmendment> GetAmendments(Type target)
		{

			if (typeof (Freezable).IsAssignableFrom(target)) {
				yield return (ITypeAmendment)typeof(FreezableAmendment<>).MakeGenericType(target).GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
			}

		}

	}
}
