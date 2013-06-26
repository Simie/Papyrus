﻿/*
 * Copyright © 2013 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;
using System.Windows;
using System.Windows.Data;
using Papyrus.Core;
using Papyrus.Studio.Framework.Controls;
using PropertyTools.Wpf;

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

			if (typeof(IRecordRef).IsAssignableFrom(property.ActualPropertyType)) {

				return CreateRecordReferenceControl(property);

			}


			if (typeof (IRecordRefCollection).IsAssignableFrom(property.ActualPropertyType)) {

				return CreateRecordReferenceListControl(property);

			}

			/*if (typeof (Papyrus.DataTypes.Color) == property.ActualPropertyType) {

				return CreatePapyrusColorControl(property);

			}*/

			return base.CreateControl(property, options);

		}

		public FrameworkElement CreateRecordReferenceControl(PropertyItem item)
		{

			var c = new RecordReferenceItem();
			c.SetBinding(RecordReferenceItem.RecordReferenceProperty, item.CreateBinding());
			return c;

		}

		public FrameworkElement CreateRecordReferenceListControl(PropertyItem item)
		{

			var c = new RecordList();
			c.SetBinding(RecordList.SourceListProperty, item.CreateBinding());
			return c;

		}

		/*public FrameworkElement CreatePapyrusColorControl(PropertyItem item)
		{
			var c = new ColorPicker2();
			var binding = item.CreateBinding();
			binding.Converter = new PapyrusColorConverter();
			c.SetBinding(ColorPicker2.SelectedColorProperty, binding);
			return c;
		}*/

	}

}
