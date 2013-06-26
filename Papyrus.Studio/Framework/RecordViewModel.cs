/*
 * Copyright © 2013 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System.ComponentModel;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Papyrus.Core;
using Papyrus.Studio.Modules.PapyrusManager;

namespace Papyrus.Studio.Framework
{
	/// <summary>
	/// Base class for any record view model. Provides Saving/Loading functionality
	/// </summary>
	public class RecordViewModel<T> : PropertyChangedBase, IRecordViewModel where T : Record
	{

		[Import] private IPapyrusManager _papyrusManager;

		private bool _isDirty;

		public virtual bool IsDirty
		{
			get { return _isDirty; }
			protected set { _isDirty = value; NotifyOfPropertyChange(() => IsDirty);}
		}

		Record IRecordViewModel.Record { get { return Record; }}

		/// <summary>
		/// Editable copy of the record
		/// </summary>
		public T Record { get; private set; }

		/// <summary>
		/// Record as fetched from the database (clean)
		/// </summary>
		protected T OriginalRecord { get; private set; }

		public string RecordID { 
			get { return Record.EditorID; }
		}

		public virtual void Open(Record record)
		{

			Record = (T)_papyrusManager.PluginComposer.GetEditableRecord(record.GetType(), record.Key);
			OriginalRecord = (T)_papyrusManager.PluginComposer.GetRecord(record.GetType(), record.Key);

			Record.PropertyChanged += RecordOnPropertyChanged;

		}

		public virtual void Close()
		{

			Record.PropertyChanged -= RecordOnPropertyChanged;

		}

		private void RecordOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{

			switch (propertyChangedEventArgs.PropertyName) {
				case "ID":
					NotifyOfPropertyChange(() => RecordID);
					break;
			}

			CheckIsDirty();

		}

		protected void CheckIsDirty()
		{

			IsDirty = !Core.Record.PropertyComparer.Equals(OriginalRecord, Record);

		}

		public void Save()
		{

			OnSaving();

			_papyrusManager.PluginComposer.SaveRecord(Record);

			OriginalRecord = (T)_papyrusManager.PluginComposer.GetRecord(Record.GetType(), Record.Key);

			CheckIsDirty();

			OnSaved();

		}

		protected virtual void OnSaving() {}
		protected virtual void OnSaved() {}
 
	}
}
