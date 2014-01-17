/*
 * Copyright © 2013 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
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

	public interface IFreezable
	{

		bool IsFrozen { get; }

		/// <summary>
		/// Set property value. Throws <c>InvalidOperationException</c> when called on a frozen object.
		/// </summary>
		void SetProperty(string name, object value);

		/// <summary>
		/// Set property value. Throws <c>InvalidOperationException</c> when called on a frozen object.
		/// </summary>
		void SetProperty<T>(Expression<Func<T>> property, T value);

	}

	public class Freezable : IFreezable
	{

		[Newtonsoft.Json.JsonIgnore]
		public bool IsFrozen { get; internal set; }


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
		/// </summary>
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

			OnPropertyChanged(member.Name);

		}

		/// <summary>
		/// Called when a property on this object is modified
		/// </summary>
		/// <param name="propName"></param>
		protected virtual void OnPropertyChanged(string propName)
		{

			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));

		}

	}
}