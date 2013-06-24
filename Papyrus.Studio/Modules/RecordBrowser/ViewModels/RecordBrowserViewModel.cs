/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Papyrus.Core;
using Papyrus.Studio.Modules.PapyrusManager;

namespace Papyrus.Studio.Modules.RecordBrowser.ViewModels
{
	[Export(typeof(IRecordBrowser))]
	public class RecordBrowserViewModel : Tool, IRecordBrowser
	{

#pragma warning disable 0649
		[Import(typeof(IPapyrusManager))]
		private IPapyrusManager _papyrusManager;
#pragma warning restore 0649

		public override PaneLocation PreferredLocation
		{
			get { return PaneLocation.Left; }
		}

		public override Uri IconSource
		{
			get { return new Uri("pack://application:,,,/Papyrus.Studio;component/Resources/Icons/Database.png"); }
		}

		private Record _selectedRecord;

		public Record SelectedRecord
		{
			get { return _selectedRecord; }
			set
			{
				_selectedRecord = value;
				NotifyOfPropertyChange(() => SelectedRecord);
			}
		}

		private IObservableCollection<Record> _activeRecords = new BindableCollection<Record>();
		/// <summary>
		/// List of the records of the currently selected type
		/// </summary>
		public Caliburn.Micro.IObservableCollection<Record> ActiveRecords
		{
			get { return _activeRecords; }
			set { _activeRecords = value; NotifyOfPropertyChange(() => ActiveRecords); }
		}

		private List<Record> _activeRecordSource = new List<Record>(); 

		private RecordTypeViewModel _selectedRecordType;
		/// <summary>
		/// Record type selected in the tree view
		/// </summary>
		public RecordTypeViewModel SelectedRecordType
		{
			get { return _selectedRecordType; }
			set
			{
				_selectedRecordType = value;
				SelectedRecordTypeName = _selectedRecordType == null ? "" : _selectedRecordType.Type.Name;
				Filter = "";
				NotifyOfPropertyChange(() => SelectedRecordType);
				UpdateActiveRecords();
				UpdateFilter();
			}
		}

		private string _selectedRecordTypeName;
		/// <summary>
		/// Name of the record typ selected in the tree view.
		/// </summary>
		public string SelectedRecordTypeName
		{
			get { return _selectedRecordTypeName; }
			set { _selectedRecordTypeName = value; NotifyOfPropertyChange(() => SelectedRecordTypeName); }
		}

		private string _filter;

		public string Filter
		{
			get { return _filter; }
			set
			{
				Console.WriteLine(_filter);
				_filter = value;
				NotifyOfPropertyChange(() => Filter);
				UpdateFilter();
			}
		}

		private void UpdateActiveRecords()
		{

			_activeRecordSource.Clear();

			if(SelectedRecordType != null)
				_activeRecordSource.AddRange(_papyrusManager.PluginComposer.GetRecords(SelectedRecordType.Type));

		}

		private void UpdateFilter()
		{

			ActiveRecords.Clear();

			if (string.IsNullOrWhiteSpace(Filter)) {
				ActiveRecords.AddRange(_activeRecordSource);
				return;
			}

			var filter = Filter.ToLower();
			ActiveRecords.AddRange(_activeRecordSource.Where(p => p.EditorID != null && p.EditorID.ToLower().Contains(filter)));

		}


		private IList<RecordTypeViewModel> _recordTypes;
		/// <summary>
		/// List of record types
		/// </summary>
		public IList<RecordTypeViewModel> RecordTypes
		{
			get { return _recordTypes; }
			set { _recordTypes = value; NotifyOfPropertyChange(() => RecordTypes); }
		}

		public RecordBrowserViewModel()
		{
			DisplayName = "Record Browser";
		}

		public void NewRecord()
		{

			if (SelectedRecordType == null)
				return;

			var newRecord = _papyrusManager.PluginComposer.CreateRecord(SelectedRecordType.Type);
			Coroutine.BeginExecute(_papyrusManager.OpenRecord(newRecord).GetEnumerator());

		}

		public void CopyRecord()
		{

			if (SelectedRecord == null)
				return;

			var newRecord = _papyrusManager.PluginComposer.GetEditableRecord(SelectedRecord.GetType(), SelectedRecord.Key);
			Coroutine.BeginExecute(_papyrusManager.OpenRecord(newRecord).GetEnumerator());

		}

		public void OpenRecord(Record record)
		{

			Coroutine.BeginExecute(_papyrusManager.OpenRecord(record).GetEnumerator());

		}

		public IEnumerable<IResult> OpenRecordWith(Record record)
		{

			yield return new SequentialResult(_papyrusManager.OpenRecordWith(record).GetEnumerator());
			
		}

		public IEnumerable<IResult> DeleteRecord(Record record)
		{

			yield break;

		} 


		private void UpdateDatabase()
		{
			SelectedRecord = null;
			SelectedRecordType = null;

			if (_papyrusManager.PluginComposer == null)
			{
				RecordTypes = new List<RecordTypeViewModel>();
				return;
			}

			var rootNode = BuildRecordTypeTree();
			RecordTypes = new List<RecordTypeViewModel>(rootNode.SubTypes);
			_papyrusManager.PluginComposer.RecordListChanged += OnRecordListChanged;

		}

		private void OnRecordListChanged(object sender, EventArgs eventArgs)
		{
			
			UpdateActiveRecords();

		}

		protected override void OnViewLoaded(object view)
		{
			base.OnViewLoaded(view);

			_papyrusManager.RecordDatabaseChanged += PapyrusManagerOnRecordDatabaseChanged;

			UpdateDatabase();

		}

		private void PapyrusManagerOnRecordDatabaseChanged(object sender, EventArgs eventArgs)
		{

			UpdateDatabase();

		}

		/// <summary>
		/// Builds a tree of record types that can be displayed using a hiararchical data template
		/// in a tree view.
		/// </summary>
		/// <returns></returns>
		static RecordTypeViewModel BuildRecordTypeTree()
		{

			var baseRecordType = new RecordTypeViewModel(typeof(Record));

			Dictionary<Type, RecordTypeViewModel> recordTypeDictionary = new Dictionary<Type, RecordTypeViewModel>();
			recordTypeDictionary.Add(typeof(Record), baseRecordType);

			var recordTypes = Papyrus.Core.Util.RecordReflectionUtil.GetRecordTypes();

			while (recordTypes.Count > 0)
			{

				var recordType = recordTypes.First();

				Type baseType = recordType.BaseType != null && recordType.BaseType.IsGenericType
					? recordType.BaseType.GetGenericTypeDefinition() : recordType.BaseType;

				if (recordTypeDictionary.ContainsKey(baseType))
				{

					var newRecordType = new RecordTypeViewModel(recordType);
					newRecordType.Visible = true;

					var parent = recordTypeDictionary[baseType];
					parent = FindVisibleParent(parent);

					parent.SubTypes.Add(newRecordType);

					newRecordType.Parent = parent;
					recordTypeDictionary[recordType] = newRecordType;
					recordTypes.RemoveAt(0);

				}
				else
				{

					// Move to the back
					recordTypes.RemoveAt(0);
					recordTypes.Add(recordType);

				}

			}

			return baseRecordType;

		}

		/// <summary>
		/// Find the nearest visible parent in the tree, stopping at root.
		/// </summary>
		/// <param name="parent"></param>
		/// <returns></returns>
		private static RecordTypeViewModel FindVisibleParent(RecordTypeViewModel parent)
		{

			while (!parent.Visible && parent.Parent != null)
				parent = parent.Parent;

			return parent;

		}

	}
}
