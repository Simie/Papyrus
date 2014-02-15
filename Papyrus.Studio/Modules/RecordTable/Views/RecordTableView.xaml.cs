using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Papyrus.Studio.Modules.RecordTable.ViewModels;
using Xceed.Wpf.DataGrid;

namespace Papyrus.Studio.Modules.RecordTable.Views
{
	/// <summary>
	/// Interaction logic for RecordTableView.xaml
	/// </summary>
	public partial class RecordTableView : UserControl
	{

		RecordTableViewModel ViewModel {get { return DataContext as RecordTableViewModel; }}

		private Type _previousRecordType;

		public RecordTableView()
		{
			InitializeComponent();
			Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			Loaded -= OnLoaded;
			ViewModel.PropertyChanged += OnPropertyChanged;
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{

			if (propertyChangedEventArgs.PropertyName == "SelectedRecordType") {
				UpdateColumns(ViewModel.SelectedRecordType);
			}

		}

		void UpdateColumns(Type recordType)
		{

			if (recordType == _previousRecordType)
				return;

			_previousRecordType = recordType;

			var props = Papyrus.Core.Util.RecordReflectionUtil.GetProperties(recordType);

			DataGrid.Columns.Clear();

			DataGrid.Columns.Add(new Column() {
				FieldName = "Key", Title = "Key", ReadOnly = true, IsMainColumn = true
			});

			foreach (var p in props) {

				DataGrid.Columns.Add(new Column() {
					FieldName = p.Name, Title = p.Name, ReadOnly = true
				});

			}

			

		}

		private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			ViewModel.OpenSelectedRecord();
		}

	}
}
