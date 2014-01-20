/*
 * Copyright © 2014 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Papyrus.Core.Util
{

	/// <summary>
	/// Utilities related to reflection
	/// </summary>
	public static class ReflectionExtensions
	{

		/// <summary>
		/// Return true if member has an attribute of type T attached
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="info"></param>
		/// <returns></returns>
		public static bool HasAttribute<T>(this MemberInfo info)
		{
			return HasAttribute(info, typeof (T));
		}

		/// <summary>
		/// Return true if member has an attribute of type attr attached
		/// </summary>
		/// <param name="info"></param>
		/// <param name="attr"></param>
		/// <returns></returns>
		public static bool HasAttribute(this MemberInfo info, Type attr)
		{
			return info.GetCustomAttributes(attr, true).Any();
		}

	}
}
