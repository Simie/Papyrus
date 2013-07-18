/*
 * Copyright © 2013 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework;
using Papyrus.Core;
using Papyrus.Studio.Modules.PapyrusManager;
using PropertyTools.Wpf;

namespace Papyrus.Studio.Framework.Controls
{

	public class RecordList : Control
	{
		
		/// <summary>
		/// The directory property.
		/// </summary>
		public static readonly DependencyProperty SourceListProperty = DependencyProperty.Register(
			"SourceList",
			typeof(IRecordRefCollection),
			typeof(RecordReferenceItem),
			new FrameworkPropertyMetadata(default(IRecordRefCollection), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SourceListChanged));

		/// <summary>
		/// Gets or sets the data pointer.
		/// </summary>
		public IRecordRefCollection SourceList
		{
			get
			{
				return (IRecordRefCollection)this.GetValue(SourceListProperty);
			}

			set
			{
				this.SetValue(SourceListProperty, value);
			}
		}

		public static readonly DependencyProperty SelectedIndexProperty =
			DependencyProperty.Register("SelectedIndex", typeof (int), typeof (RecordList), new PropertyMetadata(default(int)));

		public int SelectedIndex
		{
			get { return (int) GetValue(SelectedIndexProperty); }
			set { SetValue(SelectedIndexProperty, value); }
		}

		public static readonly DependencyProperty ListCopyProperty =
			DependencyProperty.Register("ListCopy", typeof(IObservableCollection<IRecordRef>), typeof(RecordList), new PropertyMetadata(default(IObservableCollection<IRecordRef>)));

		public IObservableCollection<IRecordRef> ListCopy
		{
			get { return (IObservableCollection<IRecordRef>)GetValue(ListCopyProperty); }
			set { SetValue(ListCopyProperty, value); }
		}

		/// <summary>
		/// Gets or sets the browse command.
		/// </summary>
		/// <value> The browse command. </value>
		public ICommand BrowseCommand { get; set; }
	
		/// <summary>
		/// Gets or sets the open command.
		/// </summary>
		/// <value> The browse command. </value>
		public ICommand OpenCommand { get; set; }

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

		static RecordList()
		{
			DefaultStyleKeyProperty.OverrideMetadata(
				typeof(RecordList), new FrameworkPropertyMetadata(typeof(RecordList)));
		}

		public RecordList()
		{
			BrowseCommand = new RelayCommand(Browse, (obj) => obj != null);
			OpenCommand = new RelayCommand(Open, (obj) => (obj as IRecordRef) != null && ((IRecordRef)(obj)).Key != RecordKey.Identity);
			MoveUpCommand = new DelegateCommand(MoveUp, MoveUpCanExecute);
			MoveDownCommand = new DelegateCommand(MoveDown, MoveDownCanExecute);
			NewItemCommand = new DelegateCommand(NewItem);
			RemoveItemCommand = new DelegateCommand(RemoveItem, () => SelectedIndex >= 0);

			Loaded += (sender, args) =>
			{
				Update();
			};
		}

		public void Init()
		{
			


		}

		public void Update()
		{
			
			if(ListCopy == null)
				ListCopy = new BindableCollection<IRecordRef>();

			ListCopy.Clear();
			ListCopy.AddRange(SourceList.References);

		}

		private static MethodInfo _applyMethodInfo;

		/// <summary>
		/// Applies changes to the source list
		/// </summary>
		private void Apply()
		{

			if (_applyMethodInfo == null) {
				_applyMethodInfo = GetType().GetMethod("ApplyInternal", BindingFlags.Instance | BindingFlags.NonPublic);
			}

			_applyMethodInfo.MakeGenericMethod(SourceList.RecordType).Invoke(this, null);

			Update();

		}

		private void ApplyInternal<T>() where T : Record
		{

			SourceList = new RecordRefCollection<T>(ListCopy.Cast<RecordRef<T>>());

		}

		private void Browse(object obj)
		{

			var index = (int) obj;

			if (!(index >= 0 && index < ListCopy.Count))
				return;

			var recordReference = ListCopy[index];

			var newEntry = RecordPicker.PickRecord(recordReference);
			ListCopy[index] = newEntry;
			Apply();


		}

		private void Open(object obj)
		{

			var recordReference = obj as IRecordRef;

			if (recordReference != null && recordReference.Key != RecordKey.Identity) {

				var papyrusManager = IoC.Get<IPapyrusManager>();
				Coroutine.BeginExecute(papyrusManager.OpenRecord(recordReference.Type, recordReference.Key).GetEnumerator());

			}

		}

		private void NewItem()
		{
			ListCopy.Add(Activator.CreateInstance(typeof(RecordRef<>).MakeGenericType(SourceList.RecordType)) as IRecordRef);
			Apply();
		}

		private void RemoveItem()
		{

			if(SelectedIndex >= 0)
				ListCopy.RemoveAt(SelectedIndex);

			Apply();

		}

		private void MoveUp()
		{

			var currentIndex = SelectedIndex;
			var item = ListCopy[currentIndex];

			var oldPointer = ListCopy[currentIndex - 1];
			ListCopy[currentIndex - 1] = item;
			ListCopy[currentIndex] = oldPointer;

			Apply();

			SelectedIndex = currentIndex - 1;

		}

		private bool MoveUpCanExecute()
		{

			return (SelectedIndex > 0);

		}

		private void MoveDown()
		{

			var currentIndex = SelectedIndex;
			var item = ListCopy[SelectedIndex];

			var oldPointer = ListCopy[currentIndex + 1];
			ListCopy[currentIndex + 1] = item;
			ListCopy[currentIndex] = oldPointer;

			Apply();
			SelectedIndex = currentIndex + 1;

		}

		private bool MoveDownCanExecute()
		{

			if (ListCopy == null)
				return false;

			return SelectedIndex < ListCopy.Count - 1;

		}

		private static void SourceListChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{

			var recordReferenceList = dependencyObject as RecordList;
			if (recordReferenceList != null)
				recordReferenceList.Init();

		}

		private static void SelectedItemChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{

			CommandManager.InvalidateRequerySuggested();

		}
		
	}

}
