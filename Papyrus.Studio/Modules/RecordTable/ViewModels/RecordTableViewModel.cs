using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Papyrus.Core;
using Papyrus.Studio.Modules.PapyrusManager;

namespace Papyrus.Studio.Modules.RecordTable.ViewModels
{
	[Export(typeof(RecordTableViewModel))]
	class RecordTableViewModel : Document
	{

		[Import]
		private IPapyrusManager _papyrusManager;

		private Type _selectedRecordType;
		private Record _selectedRecord;

		public Type SelectedRecordType
		{
			get { return _selectedRecordType; }
			set
			{
				if (value == _selectedRecordType)
					return;
				_selectedRecordType = value;
				NotifyOfPropertyChange(() => SelectedRecordType);
				RefreshRecords();
			}
		}

		public BindableCollection<Type> RecordTypes { get; private set; }

		public BindableCollection<Record> Records { get; private set; }

		public Record SelectedRecord
		{
			get { return _selectedRecord; }
			set
			{
				if (Equals(value, _selectedRecord))
					return;
				_selectedRecord = value;
				NotifyOfPropertyChange(() => SelectedRecord);
			}
		}

		protected override void OnInitialize()
		{

			base.OnInitialize();

			RecordTypes = new BindableCollection<Type>(Core.Util.RecordReflectionUtil.GetRecordTypes());
			Records = new BindableCollection<Record>();

		}

		protected override void OnViewLoaded(object view)
		{
			base.OnViewLoaded(view);
			DisplayName = "Record Tables";
		}

		private void RefreshRecords()
		{

			Records.Clear();
			Records.AddRange(_papyrusManager.PluginComposer.GetRecords(SelectedRecordType));

		}

		public void OpenSelectedRecord()
		{
			if (SelectedRecord != null) {
				OpenRecord(SelectedRecord);
			}
		}

		public void OpenRecord(Record record)
		{
			Coroutine.BeginExecute(_papyrusManager.OpenRecord(record).GetEnumerator());
		}

	}
}
