using System;
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

		/// <summary>
		/// Set property value. Throws <c>InvalidOperationException</c> when called on a frozen object.
		/// </summary>
		public void SetProperty(string name, object value)
		{
			SetPropertyInternal(GetType().GetProperty(name), value);
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
				throw new InvalidOperationException("SetProperty called on frozen record.");

			var properyInfo = member as PropertyInfo;

			if (properyInfo == null)
				throw new ArgumentException("Expected member to be property.");

			properyInfo.SetValue(this, value, null);

		}

	}
}