﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Papyrus.DataTypes;

namespace Papyrus
{
	/// <summary>
	/// Reference Article http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
	/// 
	/// Provides a method for performing a deep copy of an object.
	/// Binary Serialization is used to perform the copy.
	/// </summary>
	public static class ObjectCopier
	{
		/// <summary>
		/// Perform a deep Copy of the object.
		/// </summary>
		/// <typeparam name="T">The type of object being copied.</typeparam>
		/// <param name="source">The object instance to copy.</param>
		/// <returns>The copied object.</returns>
		public static T Clone<T>(this T source)
		{
			/*if (!Attribute.IsDefined(typeof(T), typeof(ProtoBuf.ProtoContractAttribute)))
			{
				throw new ArgumentException("The type must be a protocontract.", "source");
			}*/

			var clone = ProtoBuf.Serializer.DeepClone(source);

			if(clone is Record) {
				
				(clone as Record).ResolveDependencies((source as Record).Database);

			}

			return clone;

		}
	}

	public class DamageStructConverter : ExpandableObjectConverter
	{

		public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues)
		{

			if (context == null)
				return base.CreateInstance(context, propertyValues);
			if (context.GetType().Name.Contains("SingleSelectRootGridEntry"))
				return base.CreateInstance(context, propertyValues);

			Console.WriteLine("Create: {0}", context.ToString());
			var ret = new Damage();

			if (propertyValues.Contains("Quantity"))
				ret.Quantity = (float) propertyValues["Quantity"];
			if (propertyValues.Contains("Type"))
				ret.Type = (DamageType) propertyValues["Type"];

			return ret;
		}

		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			if (context == null)
				return base.GetCreateInstanceSupported(context);
			if (context.GetType().Name.Contains("SingleSelectRootGridEntry"))
				return base.GetCreateInstanceSupported(context);
			

			Console.WriteLine("Can: {0}", context.ToString());
			return true;
		}

	}

	public class IntSizeStructConverter : ExpandableObjectConverter
	{

		public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues)
		{

			if (context == null)
				return base.CreateInstance(context, propertyValues);
			if (context.GetType().Name.Contains("SingleSelectRootGridEntry"))
				return base.CreateInstance(context, propertyValues);

			var ret = new IntSize();

			if (propertyValues.Contains("Width"))
				ret.Width = (int)propertyValues["Width"];
			if (propertyValues.Contains("Height"))
				ret.Height = (int)propertyValues["Height"];

			return ret;
		}

		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			if (context == null)
				return base.GetCreateInstanceSupported(context);
			if (context.GetType().Name.Contains("SingleSelectRootGridEntry"))
				return base.GetCreateInstanceSupported(context);

			return true;
		}

	}

	public static class EnumExtensions
	{
		private static void CheckIsEnum<T>(bool withFlags)
		{
			if (!typeof(T).IsEnum)
				throw new ArgumentException(string.Format("Type '{0}' is not an enum", typeof(T).FullName));
			if (withFlags && !Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
				throw new ArgumentException(string.Format("Type '{0}' doesn't have the 'Flags' attribute", typeof(T).FullName));
		}

		public static bool IsFlagSet<T>(this T value, T flag) where T : struct
		{
			CheckIsEnum<T>(true);
			long lValue = Convert.ToInt64(value);
			long lFlag = Convert.ToInt64(flag);
			return (lValue & lFlag) != 0;
		}

		public static IEnumerable<T> GetFlags<T>(this T value) where T : struct
		{
			CheckIsEnum<T>(true);
			foreach (T flag in Enum.GetValues(typeof(T)).Cast<T>())
			{
				if (value.IsFlagSet(flag))
					yield return flag;
			}
		}

		public static T SetFlags<T>(this T value, T flags, bool on) where T : struct
		{
			CheckIsEnum<T>(true);
			long lValue = Convert.ToInt64(value);
			long lFlag = Convert.ToInt64(flags);
			if (on)
			{
				lValue |= lFlag;
			}
			else
			{
				lValue &= (~lFlag);
			}
			return (T)Enum.ToObject(typeof(T), lValue);
		}

		public static T SetFlags<T>(this T value, T flags) where T : struct
		{
			return value.SetFlags(flags, true);
		}

		public static T ClearFlags<T>(this T value, T flags) where T : struct
		{
			return value.SetFlags(flags, false);
		}

		public static T CombineFlags<T>(this IEnumerable<T> flags) where T : struct
		{
			CheckIsEnum<T>(true);
			long lValue = 0;
			foreach (T flag in flags)
			{
				long lFlag = Convert.ToInt64(flag);
				lValue |= lFlag;
			}
			return (T)Enum.ToObject(typeof(T), lValue);
		}

	}

}
