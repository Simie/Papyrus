using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Papyrus.Core.Util
{
	internal static class ReflectionUtil
	{
		
		/// <summary>
		/// Due to odd C# behaviour, you can only get a writable property setter when reflecting the class it is declared in.
		/// (not child classes, for example). This method walks up the type inheritance tree until if finds a writable property.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="propName"></param>
		/// <returns></returns>
		public static PropertyInfo GetWritablePropertyInfo(Type type, string propName)
		{

			PropertyInfo property = type.GetProperty(propName);

			while (property == null || !property.CanWrite) {

				type = type.BaseType;

				if (type == null)
					return null;

				property = type.GetProperty(propName);

			}

			return property;

		}


	}
}
