/*
 * Copyright © 2013 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using Papyrus.Core;
using Papyrus.Studio.Modules.PapyrusManager;

namespace Papyrus.Studio.Framework.Controls
{
	/// <summary>
	/// Interaction logic for RecordPicker.xaml
	/// </summary>
	public partial class RecordPicker : Window
	{

		private ICollection<Record> _records;
		private ICollection<Record> _filteredRecords; 

		public IRecordRef SelectedRecordReference { get; protected set; }

		public RecordPicker()
		{
			InitializeComponent();
			
		}

		private static IRecordRef CreateReference(Type t, RecordKey key)
		{
			return (IRecordRef)Activator.CreateInstance(typeof (RecordRef<>).MakeGenericType(t), key);
		}

		/// <summary>
		/// Opens the data pointer picker with the current record selected and returns the new data pointer when finished.
		/// </summary>
		/// <param name="currentPointer">Currently selected data pointer</param>
		/// <returns>Selected data pointer.</returns>
		public static IRecordRef PickRecord(IRecordRef currentPointer)
		{

			var picker = new RecordPicker();
			picker.Init(currentPointer);
			picker.WindowStartupLocation = WindowStartupLocation.Manual;

			// Position the picker at the mouse position
			picker.Loaded += (sender, args) =>
			{

				var mousePosition = picker.PointToScreen(Mouse.GetPosition(picker));
				picker.Left = mousePosition.X - picker.Width/2.0;
				picker.Top = mousePosition.Y - picker.Height/4.0;

			};

			picker.ShowDialog();

			if (!picker.DialogResult.HasValue || picker.DialogResult.Value == false)
				return currentPointer;

			var newPointer = picker.SelectedRecordReference;

			return picker.SelectedRecordReference;

		}


		private void Init(IRecordRef currentPointer)
		{

			SelectedRecordReference = currentPointer;

			_records = IoC.Get<IPapyrusManager>().PluginComposer.GetRecords(currentPointer.Type);
			_filteredRecords = _records;
			recordListBox.ItemsSource = _filteredRecords;
			recordListBox.SelectedItem = _records.FirstOrDefault(p => p.Key == currentPointer.Key);

			Title = currentPointer.Type.Name + " Picker";

		}

		private void Pick(Record result)
		{
			DialogResult = true;
			SelectedRecordReference = CreateReference(SelectedRecordReference.Type, result.Key);
			Close();
		}

		private void filterTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{

			if (string.IsNullOrWhiteSpace(filterTextBox.Text))
				_filteredRecords = _records;
			else
				_filteredRecords = _records.Where(p => p.EditorID.ToLower().Contains(filterTextBox.Text.ToLower())).ToList();

			recordListBox.ItemsSource = _filteredRecords;

		}

		private void Window_GotFocus(object sender, RoutedEventArgs e)
		{

			//filterTextBox.Focus();

		}

		private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
		{

			if(e.Key == Key.Down || e.Key == Key.Up) {

				e.Handled = true;

				if(e.Key == Key.Up && recordListBox.SelectedIndex > 0) {
					recordListBox.SelectedIndex--;
				} else if(e.Key == Key.Down && recordListBox.SelectedIndex < recordListBox.Items.Count) {
					recordListBox.SelectedIndex++;
				}
				return;

			}

			if(e.Key == Key.Enter || e.Key == Key.Return) {

				if (recordListBox.Items.Count == 1)
					recordListBox.SelectedItem = recordListBox.Items[0];
				
				if (recordListBox.SelectedItem == null)
					return;

				e.Handled = true;

				Pick(recordListBox.SelectedItem as Record);

				return;
			}

			if(filterTextBox.IsFocused == false)
				filterTextBox.Focus();

		}

		private void recordListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (recordListBox.SelectedItem != null) {
				Pick(recordListBox.SelectedItem as Record);
			}
		}

	}
}
