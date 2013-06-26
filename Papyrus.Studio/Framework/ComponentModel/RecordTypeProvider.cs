/*
 * Copyright © 2013 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
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