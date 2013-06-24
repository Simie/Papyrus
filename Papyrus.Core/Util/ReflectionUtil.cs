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
using System.Reflection;
using System.Text;

namespace Papyrus.Core.Util
{
	internal static class ReflectionUtil
	{
		
		/// <summary>
		/// Resolve a record type FullName into a Type object
		/// </summary>
		/// <param name="typeString"></param>
		/// <returns></returns>
		public static Type ResolveRecordType(string typeString)
		{

			var thisAssemblyName = Assembly.GetExecutingAssembly().FullName;

			// Get assemblies, filtered to those than contain Papyrus.Core.dll as a reference
			var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(p => p.GetReferencedAssemblies().Any(q => q.FullName == thisAssemblyName));

			// TODO: Cache type results or specify loaded modules somewhere
			return assemblies.Select(assembly => assembly.GetType(typeString, false, false)).FirstOrDefault(type => type != null);

		}

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
