using System;
using System.ComponentModel;
using Papyrus.Core;

namespace Papyrus.Studio.Framework.ComponentModel
{
	public class RecordTypeProvider : TypeDescriptionProvider
	{

		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{

			if(objectType.IsSubclassOf(typeof(Record)))
				return new RecordTypeDescriptor(objectType);

			return base.GetTypeDescriptor(objectType, instance);

		}

	}
}