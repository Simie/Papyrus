using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Afterthought;

namespace Papyrus.Core.Build
{

	public static class FreezableAmendMethods<TAmended> where TAmended : Freezable
	{

		/// <summary>
		/// Property amendment, called before a record property is set. Throws an exception if record is frozen
		/// </summary>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="instance"></param>
		/// <param name="propertyName"></param>
		/// <param name="oldValue"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static TProperty BeforePropertySet<TProperty>(TAmended instance, string propertyName, TProperty oldValue,
			TProperty value)
		{

			if (instance.IsFrozen)
				throw new InvalidOperationException("Attempted to modify read-only (frozen) property.");

			return value;

		}

		/// <summary>
		/// Triggers OnPropertyChanged event when a property is modified
		/// </summary>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="instance"></param>
		/// <param name="propertyName"></param>
		/// <param name="oldValue"></param>
		/// <param name="value"></param>
		/// <param name="newValue"></param>
		public static void AfterPropertySet<TProperty>(TAmended instance, string propertyName, TProperty oldValue,
			TProperty value, TProperty newValue)
		{

			if ((oldValue == null ^ newValue == null) || (oldValue != null && !oldValue.Equals(newValue)))
				instance.OnPropertyChanged(propertyName);

		}


	}

	class FreezableAmendment<T> : Amendment<T,T> where T : Freezable
	{

		public override void Amend<TProperty>(Property<TProperty> property)
		{

			if (property.IsAmended || !property.PropertyInfo.CanWrite)
				return;

			if (property.Name == "IsFrozen")
				return;

			Console.WriteLine("Amending {0} ({1})", property.Name, typeof (T));

			property.BeforeSet = FreezableAmendMethods<T>.BeforePropertySet;
			property.AfterSet = FreezableAmendMethods<T>.AfterPropertySet;

			base.Amend(property);
		}

	}

}
