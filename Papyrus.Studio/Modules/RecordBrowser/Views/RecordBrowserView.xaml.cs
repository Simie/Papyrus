/*
 * Copyright © 2014 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;
using System.Windows;
using System.Windows.Controls;
using Papyrus.Core;
using Papyrus.Studio.Modules.RecordBrowser.ViewModels;

namespace Papyrus.Studio.Modules.RecordBrowser.Views
{
	/// <summary>
	/// Interaction logic for RecordBrowser.xaml
	/// </summary>
	public partial class RecordBrowserView : UserControl
	{

		public RecordBrowserViewModel ViewModel { get; private set; }

		public RecordBrowserView()
		{
			InitializeComponent();
			Loaded += OnLoaded;
		
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			ViewModel = (DataContext as RecordBrowserViewModel);
		}


		private void RecordTypes_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			ViewModel.SelectedRecordType = e.NewValue as RecordTypeViewModel;
		}

		private void OnDataGridDoubleClick(object sender, EventArgs e)
		{

			//(DataContext as RecordBrowserViewModel).SelectedRecordType = e.NewValue as RecordTypeViewModel;

			if(ViewModel.SelectedRecord != null)
				ViewModel.OpenRecord(ViewModel.SelectedRecord);

		}

		private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
		{

			ViewModel.SelectedRecord = RecordGrid.SelectedItem as Record;

		}

	}
}
