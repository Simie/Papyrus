using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;

namespace Papyrus.Studio.Framework.Controls
{
	public class EnumFlagsControl : Control
	{

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
			"Value", typeof (Enum), typeof (EnumFlagsControl), new PropertyMetadata(default(Enum), ValuePropertyChanged));

		public Enum Value
		{
			get { return (Enum) GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		#region EnumOption

		private static readonly DependencyPropertyKey ItemsPropertyKey = DependencyProperty.RegisterReadOnly("Items",
			typeof (ObservableCollection<EnumOption>), typeof (EnumFlagsControl), new PropertyMetadata(null));

		public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

		public ObservableCollection<EnumOption> Items
		{
			get { return (ObservableCollection<EnumOption>) GetValue(ItemsProperty); }
			set { SetValue(ItemsPropertyKey, value); }
		}

		public class EnumOption : PropertyChangedBase
		{

			private string _displayName;
			private bool _isSelected;

			public string DisplayName
			{
				get { return _displayName; }
				set
				{
					if (value == _displayName)
						return;
					_displayName = value;
					NotifyOfPropertyChange(() => DisplayName);
				}
			}

			public bool IsSelected
			{
				get { return _isSelected; }
				set
				{
					if (value.Equals(_isSelected))
						return;
					_isSelected = value;
					Debug.WriteLine("{0} IsSelected: {1}", DisplayName, IsSelected);
					NotifyOfPropertyChange(() => IsSelected);
				}
			}

			public ulong Value { get; set; }

		}

		#endregion

		static EnumFlagsControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(EnumFlagsControl), new FrameworkPropertyMetadata(typeof(EnumFlagsControl)));
		}

		protected ulong ValueData
		{
			get { return Convert.ToUInt64(Convert.ChangeType(Value, Value.GetTypeCode())); }
			set { Value = (Enum)Enum.ToObject(Value.GetType(), value); }
		}

		protected void OnValueChanged(Enum oldValue, Enum newValue)
		{

			Debug.WriteLine("New Value: {0}", Value);

			if (_isUpdating)
				return;

			ClearOptions();
			UpdateOptions();
			
		}

		protected void ClearOptions()
		{

			if (Items == null) {
				Items = new BindableCollection<EnumOption>();
				return;
			}

			foreach (var op in Items) {
				op.PropertyChanged -= OptionIsSelectedChanged;
			}

			Items.Clear();

		}

		private void OptionIsSelectedChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{

			if (_isUpdating)
				return;

			var op = sender as EnumOption;
			PersistChange(op.Value, op.IsSelected);

			UpdateOptions();

		}

		private bool _isUpdating;

		protected void UpdateOptions()
		{

			if (_isUpdating)
				return;

			Debug.WriteLine("UpdateOptions");

			_isUpdating = true;

			var names = Value.GetType().GetEnumNames();
			var values = Value.GetType().GetEnumValues();

			var value = ValueData;

			for (int i = 0; i < names.Length; i++) {

				var name = names[i];
				var v = Convert.ToUInt64(values.GetValue(i));
				
				var isSelected = (((value) & v) == v && v > 0) || value == v;

				if (Items.Count <= i) {


					var op = new EnumOption() {
						DisplayName = name,
						IsSelected = isSelected,
						Value = v
					};

					Items.Add(op);

					op.PropertyChanged += OptionIsSelectedChanged;

				} else {

					Items[i].IsSelected = isSelected;

				}

			}

			_isUpdating = false;

		}

		protected void PersistChange(ulong flag, bool value)
		{

			Debug.WriteLine("PersistChange: {0} {1}", flag, value);

			if (flag == 0) {
				ValueData = 0;
				return;
			}

			ulong v = ValueData;

			if (value)
				v |= flag;
			else
				v &= ~flag;

			ValueData = v;

		}

		private static void ValuePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
		{

			var d = dependencyObject as EnumFlagsControl;
			d.OnValueChanged((Enum)args.OldValue, (Enum)args.NewValue);

		}

	}
}
