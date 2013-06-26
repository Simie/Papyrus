/*
 * Copyright © 2013 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
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
