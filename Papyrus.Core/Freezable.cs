/*
 * Copyright © 2014 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Papyrus.Core.Util;

namespace Papyrus.Core
{

	/// <summary>
	/// Represents an object which can be frozen (properties made read-only)
	/// </summary>
	public interface IFreezable
	{

		/// <summary>
		/// If true, attempting to change a value on this record will result in an InvalidOperationException being thrown.
		/// </summary>
		bool IsFrozen { get; }

		/// <summary>
		/// Set property value. Throws <c>InvalidOperationException</c> when called on a frozen object.
		/// </summary>
		void SetProperty(string name, object value);

		/// <summary>
		/// Set property value. Throws <c>InvalidOperationException</c> when called on a frozen object.
		/// </summary>
		[Obsolete]
		void SetProperty<T>(Expression<Func<T>> property, T value);

	}
	/// <summary>
	/// Base class for an object which can be frozen (properties made read-only)
	/// </summary>
	public class Freezable : IFreezable
	{

		/// <summary>
		/// If true, attempting to change a value on this record will result in an InvalidOperationException being thrown.
		/// </summary>
		[Newtonsoft.Json.JsonIgnore]
		[Browsable(false)]
		public bool IsFrozen { get; internal set; }

		/// <summary>
		/// Invoked whenever a property is changed (usually only when editing records)
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Set property value. Throws <c>InvalidOperationException</c> when called on a frozen object.
		/// </summary>
		public void SetProperty(string name, object value)
		{
			SetPropertyInternal(ReflectionUtil.GetWritablePropertyInfo(GetType(), name), value);
		}

		/// <summary>
		/// Set property value. Throws <c>InvalidOperationException</c> when called on a frozen object.
		/// Deprecated, strongly typed property setting can now use normal property setter syntax
		/// </summary>
		[Obsolete("Set property directly.")]
		public void SetProperty<T>(Expression<Func<T>> property, T value)
		{
			SetPropertyInternal(property.GetMemberInfo(), value);
		}

		private void SetPropertyInternal(MemberInfo member, object value)
		{

			if (IsFrozen)
				throw new InvalidOperationException("SetProperty called on frozen object.");

			var properyInfo = member as PropertyInfo;

			if (properyInfo == null)
				throw new ArgumentException("Expected member to be property.");

			properyInfo.SetValue(this, value, null);
			
		}

		/// <summary>
		/// Called when a property on this object is modified
		/// </summary>
		/// <param name="propName"></param>
		public virtual void OnPropertyChanged(string propName)
		{

			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));

		}

	}
}