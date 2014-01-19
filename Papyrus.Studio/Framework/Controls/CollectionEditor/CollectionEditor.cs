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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using PropertyTools.Wpf;

namespace Papyrus.Studio.Framework.Controls
{

	public class CollectionEditor : Control
	{

		public static readonly DependencyProperty ItemsProperty =
			DependencyProperty.Register("Items", typeof (IObservableCollection<object>), typeof (CollectionEditor), new PropertyMetadata(default(IObservableCollection<object>)));

		public IObservableCollection<object> Items
		{
			get { return (IObservableCollection<object>) GetValue(ItemsProperty); }
			set { SetValue(ItemsProperty, value); }
		}

		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register("ItemsSource", typeof (ICollection), typeof (CollectionEditor), new PropertyMetadata(default(IList), ItemsSourceChanged));

		public ICollection ItemsSource
		{
			get { return (ICollection) GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register("SelectedItem", typeof (object), typeof (CollectionEditor), new PropertyMetadata(default(object)));


		public object SelectedItem
		{
			get { return (object) GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		public static readonly DependencyProperty SelectedIndexProperty =
			DependencyProperty.Register("SelectedIndex", typeof (int), typeof (CollectionEditor), new PropertyMetadata(default(int), SelectedIndexChanged));

		public int SelectedIndex
		{
			get { return (int) GetValue(SelectedIndexProperty); }
			set { SetValue(SelectedIndexProperty, value); }
		}

		public static readonly DependencyProperty NewItemsSourceProperty =
			DependencyProperty.Register("NewItemsSource", typeof (List<Type>), typeof (CollectionEditor), new PropertyMetadata(default(List<Type>)));

		public List<Type> NewItemsSource
		{
			get { return (List<Type>) GetValue(NewItemsSourceProperty); }
			set { SetValue(NewItemsSourceProperty, value); }
		}

		public static readonly DependencyProperty NewTypeSelectedProperty =
			DependencyProperty.Register("NewTypeSelected", typeof (Type), typeof (CollectionEditor), new PropertyMetadata(default(Type)));

		public Type NewTypeSelected
		{
			get { return (Type) GetValue(NewTypeSelectedProperty); }
			set { SetValue(NewTypeSelectedProperty, value); }
		}

		/// <summary>
		/// Gets or sets the new item command
		/// </summary>
		public ICommand NewItemCommand { get; set; }

		/// <summary>
		/// Gets or sets the remove item command
		/// </summary>
		public ICommand RemoveItemCommand { get; set; }

		/// <summary>
		/// Gets or sets the move up command
		/// </summary>
		public ICommand MoveUpCommand { get; set; }

		/// <summary>
		/// Gets or sets the move down command
		/// </summary>
		public ICommand MoveDownCommand { get; set; }

		static CollectionEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(
				typeof(CollectionEditor), new FrameworkPropertyMetadata(typeof(CollectionEditor)));
		}

		public CollectionEditor()
		{

			MoveUpCommand = new DelegateCommand(MoveUpExecuted, CanMoveUpExecute);
			MoveDownCommand = new DelegateCommand(MoveDownExecuted, CanMoveDownExecute);
			NewItemCommand = new DelegateCommand(NewCommandExecuted, NewItemCanExecute);
			RemoveItemCommand = new DelegateCommand(DeleteCommandExecuted, DeleteCanExecute);

			/*CommandBindings.Add(new CommandBinding(ApplicationCommands.New, NewCommandExecuted, NewItemCanExecute));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, DeleteCommandExecuted, DeleteCanExecute));
			CommandBindings.Add(new CommandBinding(ComponentCommands.MoveUp, MoveUpExecuted, CanMoveUpExecute));
			CommandBindings.Add(new CommandBinding(ComponentCommands.MoveUp, MoveDownExecuted, CanMoveDownExecute));*/

			Loaded += OnLoaded;

		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{

			if (ItemsSource == null)
				ItemsSource = new List<Object>();

			if (NewItemsSource == null) {

				var parent = ItemsSource.GetType().GetGenericArguments()[0];

				if (!parent.IsAbstract) {
					NewItemsSource = new List<Type>() {parent};
				} else {
					NewItemsSource = parent.Assembly.GetTypes().Where(type => type.IsSubclassOf(parent) && !type.IsAbstract).ToList();
				}

			}

			NewTypeSelected = NewItemsSource[0];

			UpdateItems();

			// Hack to fix control factory not being applied correctly
			var propGrid = UIHelper.FindChild<PropertyControl>(this, "_propGrid");
			propGrid.PropertyControlFactory = PapyrusPropertyControlFactory.GetControlFactory();
			
			// Hack to have items refreshing correctly when a property changes in the grid
			var listView = UIHelper.FindChild<ListBox>(this, "_itemList");
			propGrid.PreviewKeyUp += (o, args) => listView.Items.Refresh();
			propGrid.PreviewMouseUp += (o, args) => listView.Items.Refresh();

		}

		private void PersistChanges()
		{

			var type = ItemsSource.GetType();

			var valueType = type.GetGenericArguments()[0];

			dynamic list = Activator.CreateInstance(typeof(List<>).MakeGenericType(valueType));

			foreach (var item in Items.Cast<dynamic>()) {
				list.Add(item);
			}
			
			ItemsSource = (ICollection) Activator.CreateInstance(type, list);

		}

		private object NewItem(Type type)
		{
			return Activator.CreateInstance(type);
		}

		private void UpdateItems()
		{
			
			if(Items == null)
				Items = new BindableCollection<object>();
			
			Items.Clear();

			if (ItemsSource == null)
				ItemsSource = new List<object>();

			foreach (var item in ItemsSource) {
				Items.Add(item);
			}

			SelectedIndex = -1;
			//SelectedItem = null;

		}

		private bool CanMoveUpExecute()
		{

			if (SelectedIndex > 0)
				return true;
			return false;

		}

		private void MoveUpExecuted()
		{

			if (!CanMoveUpExecute())
				return;

			//var selectedItem = SelectedItem;
			var item = Items[SelectedIndex];
			var index = SelectedIndex;
			Items.RemoveAt(index);
			Items.Insert(index-1, item);

			PersistChanges();

			SelectedIndex = index - 1;
			//SelectedItem = selectedItem;

		}

		private bool CanMoveDownExecute()
		{

			if (Items != null && SelectedIndex < Items.Count-1)
				return true;
			return false;

		}

		private void MoveDownExecuted()
		{

			if (!CanMoveDownExecute())
				return;

			var item = Items[SelectedIndex];
			var index = SelectedIndex;
			Items.RemoveAt(index);
			Items.Insert(index + 1, item);

			PersistChanges();

			SelectedIndex = index + 1;

		}

		private bool DeleteCanExecute()
		{
			return SelectedIndex >= 0;
		}

		private void DeleteCommandExecuted()
		{
			
			if(SelectedIndex >= 0)
				Items.RemoveAt(SelectedIndex);

			PersistChanges();

		}

		private void NewCommandExecuted()
		{

			if (NewTypeSelected == null)
				return;

			var newItem = NewItem(NewTypeSelected);
			Items.Add(newItem);
			PersistChanges();
			SelectedIndex = Items.Count - 1;

		}

		private bool NewItemCanExecute()
		{

			//return true;
			return NewTypeSelected != null;

		}

		private static void SelectedIndexChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{

			var c = (CollectionEditor) dependencyObject;
			dependencyObject.SetValue(SelectedItemProperty, c.SelectedIndex >= 0 ? c.Items[c.SelectedIndex] : null);
			CommandManager.InvalidateRequerySuggested();

		}

		private static void ItemsSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{

			var editor = dependencyObject as CollectionEditor;

			if (editor != null) {
				editor.UpdateItems();
			}

		}

	}

}
