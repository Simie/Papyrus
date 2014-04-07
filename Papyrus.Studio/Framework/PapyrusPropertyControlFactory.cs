/*
 * Copyright © 2014 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Caliburn.Micro;
using Papyrus.Core;
using Papyrus.Studio.Framework.Controls;
using Papyrus.Studio.Framework.Services;
using PropertyTools.Wpf;
using Xceed.Wpf.Toolkit;

namespace Papyrus.Studio.Framework
{

	public class PapyrusPropertyControlFactory : DefaultPropertyControlFactory
	{

		public static Type FactoryType = typeof(PapyrusPropertyControlFactory);

		public static IPropertyControlFactory GetControlFactory()
		{
			return Activator.CreateInstance(FactoryType) as IPropertyControlFactory;
		}

		public PapyrusPropertyControlFactory()
		{
			
		}

		public override System.Windows.FrameworkElement CreateControl(PropertyItem property, PropertyControlFactoryOptions options)
		{

			var factories = IoC.GetAll<IPropertyControlProvider>();

			FrameworkElement e = null;

			return factories.Any(factory => (e = factory.CreateControl(property, options)) != null) ? e : base.CreateControl(property, options);

		}

	}

}
