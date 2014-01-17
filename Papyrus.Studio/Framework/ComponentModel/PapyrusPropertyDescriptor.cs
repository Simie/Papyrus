/*
 * Copyright © 2014 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Papyrus.Core;

namespace Papyrus.Studio.Framework
{
	public class PapyrusPropertyDescriptor : PropertyDescriptor
	{

		private readonly Type _propertyType;
		private readonly Type _ownerType;

		private readonly PropertyInfo _propertyInfo;

		private readonly Dictionary<Record, List<EventHandler>> _valueChangedSubscribers = new Dictionary<Record, List<EventHandler>>(); 

		public PapyrusPropertyDescriptor(Type ownerType, string name, Type propertyType)
			: base(name, null)
		{

			_ownerType = ownerType;
			_propertyType = propertyType;
			_propertyInfo = ownerType.GetProperty(name);

		}

		protected override void FillAttributes(System.Collections.IList attributeList)
		{

			foreach (var attrib in _propertyInfo.GetCustomAttributes(true)) {
				attributeList.Add(attrib);
			}

			base.FillAttributes(attributeList);

		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override Type ComponentType
		{
			get { return _ownerType; }
		}

		public override object GetValue(object component)
		{
			return _propertyInfo.GetValue(component, null);
		}

		public override bool IsReadOnly
		{
			get { return false; }
		}

		public override Type PropertyType
		{
			get { return _propertyType; }
		}

		public override void SetValue(object component, object value)
		{
			var oldValue = GetValue(component);

			if (oldValue != value) {
				var record = (Record)component;
				record.SetProperty(Name, value);
				OnValueChanged(component, new PropertyChangedEventArgs(Name));
			}
		}

		public override void ResetValue(object component)
		{
			throw new NotImplementedException();
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}

		public override void AddValueChanged(object component, EventHandler handler)
		{

			base.AddValueChanged(component, handler);

			var record = component as Record;

			if (record != null) {

				List<EventHandler> handlerList;

				if (!_valueChangedSubscribers.TryGetValue(record, out handlerList)) {

					_valueChangedSubscribers.Add(record, new List<EventHandler>() {handler});

					record.PropertyChanged += RecordOnPropertyChanged;

				} else {

					handlerList.Add(handler);

				}

			}

		}

		public override void RemoveValueChanged(object component, EventHandler handler)
		{

			base.RemoveValueChanged(component, handler);

			var record = component as Record;

			if (record != null) {

				List<EventHandler> handlerList;

				if (!_valueChangedSubscribers.TryGetValue(record, out handlerList)) {

					record.PropertyChanged -= RecordOnPropertyChanged;

				} else {

					handlerList.Remove(handler);

					if (handlerList.Count == 0)
						record.PropertyChanged -= RecordOnPropertyChanged;
					_valueChangedSubscribers.Remove(record);

				}

			}

		}

		private void RecordOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{

			if (propertyChangedEventArgs.PropertyName != Name)
				return;

			var record = sender as Record;

			if (record == null)
				return;

			var subscribers = _valueChangedSubscribers[record];

			if(subscribers != null)
				subscribers.ForEach(p => p.Invoke(this, EventArgs.Empty));

		}

	}
}