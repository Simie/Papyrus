using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Papyrus.Studio.Framework
{
	class PapyrusPropertyItem : PropertyTools.Wpf.PropertyItem
	{

		public PapyrusPropertyItem(PropertyDescriptor propertyDescriptor, PropertyDescriptorCollection propertyDescriptors) : base(propertyDescriptor, propertyDescriptors) {}

		public override System.Windows.Data.Binding CreateBinding(System.Windows.Data.UpdateSourceTrigger trigger = System.Windows.Data.UpdateSourceTrigger.Default)
		{

			var bindingMode = this.Descriptor.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
			var formatString = this.FormatString;
			if (formatString != null && !formatString.StartsWith("{")) {
				formatString = "{0:" + formatString + "}";
			}

			

			var binding = new Binding(this.PropertyName) {
				Mode = bindingMode,
				Converter = this.Converter,
				ConverterParameter = this.ConverterParameter,
				StringFormat = formatString,
				UpdateSourceTrigger = trigger,
				ValidatesOnDataErrors = true,
				ValidatesOnExceptions = true
			};
			if (this.ConverterCulture != null) {
				binding.ConverterCulture = this.ConverterCulture;
			}

			return binding;

		}

	}
}
