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