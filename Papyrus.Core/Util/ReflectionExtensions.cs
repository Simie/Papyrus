using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Papyrus.Core.Util
{
	public static class ReflectionExtensions
	{

		public static bool HasAttribute<T>(this MemberInfo info)
		{
			return HasAttribute(info, typeof (T));
		}

		public static bool HasAttribute(this MemberInfo info, Type attr)
		{
			return info.GetCustomAttributes(attr, true).Any();
		}

	}
}
