﻿/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Papyrus.DataTypes;

namespace Papyrus.Design
{
	class DataPointerConverter : TypeConverter
	{

		// TODO: Data format needs to be modified to have datapointers have references to their target data before we can do this.
		/*
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return false;
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{

			if (destinationType == typeof(string))
				return true;

			return false;

		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			
			if(destinationType == typeof(string)) {

				var pointerType = value.GetType().GetGenericArguments()[0];
				var index = (uint) value.GetType().GetProperty("Index").GetValue(value, null);
				
				if(index == 0) {
					return "None";
				}

				Record data = Editor.Instance.GetObjectFromDataList(index, pointerType) as Record;

				if(data == null)
					throw new Exception();

				return "(" + index + ") " + data.ID;
			}
			
			return base.ConvertTo(context, culture, value, destinationType);
		}
		*/
	}
}