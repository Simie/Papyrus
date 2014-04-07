using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using PropertyTools.Wpf;

namespace Papyrus.Studio.Framework.Services
{

	public interface IPropertyControlProvider
	{

		FrameworkElement CreateControl(PropertyItem property, PropertyControlFactoryOptions options);

	}

}
