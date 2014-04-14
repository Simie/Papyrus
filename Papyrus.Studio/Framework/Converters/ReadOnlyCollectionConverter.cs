using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Papyrus.Studio.Framework.Converters
{

	public class ReadOnlyCollectionConverter: IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{

			if (!(value is ICollection))
				return Binding.DoNothing;

			dynamic source = value;

			dynamic list =
				Activator.CreateInstance(typeof (List<>).MakeGenericType(value.GetType().GetGenericArguments().First()));

			foreach (var item in source) {
				list.Add(item);
			}

			return list;

		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			
			throw new NotSupportedException();

		}

	}
}
