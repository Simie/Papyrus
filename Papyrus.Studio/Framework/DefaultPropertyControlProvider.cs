using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using Papyrus.Studio.Framework.Controls;
using Papyrus.Studio.Framework.Converters;
using Papyrus.Studio.Framework.Services;
using PropertyTools.Wpf;

namespace Papyrus.Studio.Framework
{
	[Export(typeof(IPropertyControlProvider))]
	class DefaultPropertyControlProvider : IPropertyControlProvider
	{

		public FrameworkElement CreateControl(PropertyItem property, PropertyControlFactoryOptions options)
		{

			if (typeof(ICollection).IsAssignableFrom(property.ActualPropertyType)) {
				return CreateCollectionControl(property);
			}

			if (property.ActualPropertyType.IsEnum && property.ActualPropertyType.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0) {
				return CreateEnumFlagsControl(property);
			}

			return null;

		}

		public FrameworkElement CreateCollectionControl(PropertyItem item)
		{

			var c = new CollectionEditor();
			c.SetBinding(CollectionEditor.ItemsSourceProperty, item.CreateBinding());
			return c;

		}

		public FrameworkElement CreateEnumFlagsControl(PropertyItem item)
		{

			var c = new EnumFlagsControl();
			var binding = item.CreateBinding();
			binding.Converter = new ObjectTypeConverter();
			c.SetBinding(EnumFlagsControl.ValueProperty, binding);
			return c;

		}

	}
}
