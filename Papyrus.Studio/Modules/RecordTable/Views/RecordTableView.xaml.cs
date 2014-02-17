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
using Papyrus.Core;
using Papyrus.Studio.Framework.Converters;
using Papyrus.Studio.Modules.RecordTable.ViewModels;
using Xceed.Wpf.DataGrid;

namespace Papyrus.Studio.Modules.RecordTable.Views
{
	/// <summary>
	/// Interaction logic for RecordTableView.xaml
	/// </summary>
	public partial class RecordTableView : UserControl
	{

		private RecordRefNameConverter _nameConverter;

		RecordTableViewModel ViewModel {get { return DataContext as RecordTableViewModel; }}

		private Type _previousRecordType;

		public RecordTableView()
		{
			InitializeComponent();
			Loaded += OnLoaded;
			_nameConverter = new RecordRefNameConverter();
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

		private static readonly ICollection<Type> PermittedSortTypes = new[] {
			typeof(string),
			typeof(int),
			typeof(double),
			typeof(float),
			typeof(Enum)
		};

		private static readonly ICollection<Type> ExcludedTypes = new[] {
			typeof (RecordRefCollection<>),
			typeof (ReadOnlyCollection<>),
			typeof(ICollection<>),
			typeof(IList<>)
		};

		void UpdateColumns(Type recordType)
		{

			if (recordType == _previousRecordType)
				return;

			_previousRecordType = recordType;

			var props = Papyrus.Core.Util.RecordReflectionUtil.GetProperties(recordType);

			DataGrid.Columns.Clear();

			DataGrid.Columns.Add(new Column() {
				FieldName = "Key", Title = "Key", ReadOnly = true, IsMainColumn = true, AllowSort = false
			});

			foreach (var p in props) {

				if(ExcludedTypes.Any(q => q.IsAssignableFrom(p.PropertyType)))
					continue;

				if (p.PropertyType.IsGenericType) {
					if(ExcludedTypes.Any(q => q.IsAssignableFrom(p.PropertyType.GetGenericTypeDefinition())))
						continue;
				}

				var canSort = PermittedSortTypes.Contains(p.PropertyType);

				var column = new Column() {
					FieldName = p.Name, Title = p.Name, ReadOnly = true, AllowSort = canSort
				};

				if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof (RecordRef<>)) {
					
					column.DisplayMemberBindingInfo = new DataGridBindingInfo() {
						Path = new PropertyPath(p.Name),
						Converter = _nameConverter, ReadOnly = true
					};

				}

				DataGrid.Columns.Add(column);

			}

			

		}

		private void RowDoubleClickHandler(object sender, MouseButtonEventArgs e)
		{

			var row = e.Source as DataRow;

			if (row == null) {
				ViewModel.OpenSelectedRecord();
				return;
			}

			var record = row.DataContext as Record;

			ViewModel.OpenRecord(record);

		}

	}
}
