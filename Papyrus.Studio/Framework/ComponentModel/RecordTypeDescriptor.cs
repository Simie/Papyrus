/*
 * Copyright © 2013 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.ComponentModel;
using System.Linq;
using Papyrus.Core.Util;

namespace Papyrus.Studio.Framework.ComponentModel
{
	public class RecordTypeDescriptor : CustomTypeDescriptor
	{

		private readonly Type _recordType;

		private readonly PropertyDescriptor[] _propertyDescriptors;

		public RecordTypeDescriptor(Type recordType)
		{
			_recordType = recordType;

			var properties = RecordReflectionUtil.GetProperties(_recordType);

			_propertyDescriptors = properties.Select(p => new PapyrusPropertyDescriptor(_recordType, p.Name, p.PropertyType)).Cast<PropertyDescriptor>().ToArray();

		}

		public override PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(null);
		}

		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{

			return new PropertyDescriptorCollection(_propertyDescriptors);

		}

	}
}