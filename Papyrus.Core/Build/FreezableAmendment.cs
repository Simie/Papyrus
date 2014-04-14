using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Afterthought;

namespace Papyrus.Core.Build
{

	static class FreezableAmendMethods<TAmended> where TAmended : Freezable
	{

		public static TProperty BeforePropertySet<TProperty>(TAmended instance, string propertyName, TProperty oldValue,
			TProperty value)
		{

			if (instance.IsFrozen)
				throw new InvalidOperationException("Attempted to modify read-only (frozen) property.");

			return value;

		}

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
