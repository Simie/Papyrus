/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
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

		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register("SelectedItem", typeof (IRecordRef), typeof (RecordList), new PropertyMetadata(default(IRecordRef), SelectedItemChanged));


		public IRecordRef SelectedItem
		{
			get { return (IRecordRef)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
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
			RemoveItemCommand = new DelegateCommand(RemoveItem, () => SelectedItem != null);

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

		private void Browse(object obj)
		{

			var recordReference = obj as IRecordRef;

			if (recordReference != null) {

				var index = SourceList.References.IndexOf(recordReference);

				/*if(index >= 0)
					SourceList[index] = Papyrus.Studio.Controls.RecordPicker.PickRecord(recordReference);
				*/
				Update();

			}

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
			//SourceList.Add(Activator.CreateInstance(typeof(RecordRef<>).MakeGenericType(SourceList.RecordType)) as IRecordRef);
			Update();
		}

		private void RemoveItem()
		{

			/*if(SelectedItem != null)
				SourceList.Remove(SelectedItem);*/
			Update();

		}

		private void MoveUp()
		{

			/*var currentIndex = SourceList.References.IndexOf(SelectedItem);
			var item = SelectedItem;

			var oldPointer = SourceList[currentIndex - 1];
			SourceList[currentIndex - 1] = SelectedItem;
			SourceList[currentIndex] = oldPointer;

			Update();

			SelectedItem = item;*/

		}

		private bool MoveUpCanExecute()
		{
			/*if (SelectedItem == null)
				return false;

			var currentIndex = SourceList.IndexOf(SelectedItem);

			if (currentIndex < 1)
				return false;*/


			return true;
		}

		private void MoveDown()
		{

			/*var currentIndex = SourceList.IndexOf(SelectedItem);
			var item = SelectedItem;

			var oldPointer = SourceList[currentIndex + 1];
			SourceList[currentIndex + 1] = SelectedItem;
			SourceList[currentIndex] = oldPointer;

			Update();
			SelectedItem = item;*/

		}

		private bool MoveDownCanExecute()
		{
			/*if (SelectedItem == null)
				return false;

			var currentIndex = SourceList.IndexOf(SelectedItem);

			if (currentIndex >= SourceList.Count - 1)
				return false;*/


			return true;
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
