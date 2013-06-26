/*
 * Copyright © 2013 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System.Collections.Generic;
using System.ComponentModel;
using Caliburn.Micro;
using Papyrus.Core;
using Papyrus.Studio.Framework;
using Gemini.Framework;

namespace Papyrus.Studio.Modules.GenericRecordEditor.ViewModels
{
	/// <summary>
	/// Generic record editor view model
	/// </summary>
	public class GenericEditorViewModel : Document, ISaveAware, IRecordDocument
	{

		public Record Record { get { return RecordModel.Record; } }

		private GenericRecordViewModel _recordModel;
		public GenericRecordViewModel RecordModel
		{
			get { return _recordModel; }
			protected set { _recordModel = value; NotifyOfPropertyChange(() => RecordModel); }
		}

		public bool IsDirty
		{
			get { return RecordModel.IsDirty; }
		}

		protected override void OnViewLoaded(object view)
		{
			base.OnViewLoaded(view);
			UpdateDisplayName();
		}

		public override void CanClose(System.Action<bool> callback)
		{

			if (!IsDirty) {

				callback(true);
				return;

			}

			var result = SaveUtil.ShowSaveDialog(RecordModel.RecordID);

			if (result == SaveUtil.SaveDialogResult.Cancel) {
				callback(false);
				return;
			}

			if (result == SaveUtil.SaveDialogResult.Save)
				Coroutine.BeginExecute(Save().GetEnumerator());

			callback(true);

			RecordModel.Close();

		}

		void UpdateDisplayName()
		{

			var baseName = string.IsNullOrEmpty(RecordModel.RecordID) ? "[Unnamed]" : RecordModel.RecordID;

			if (IsDirty)
				baseName = baseName + "*";

			DisplayName = baseName;

		}

		protected override void OnActivate()
		{

			base.OnActivate();

			UpdateDisplayName();

		}

		protected override void OnDeactivate(bool close)
		{

			if (close) {

				RecordModel.PropertyChanged -= RecordModelOnPropertyChanged;

			}

			base.OnDeactivate(close);

		}

		public void Open(Record record)
		{

			RecordModel = new GenericRecordViewModel();
			IoC.BuildUp(RecordModel);
			RecordModel.Open(record);

			RecordModel.PropertyChanged += RecordModelOnPropertyChanged;

		}

		private void RecordModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{

			switch (propertyChangedEventArgs.PropertyName) {
				case "RecordID":
					UpdateDisplayName();
					break;
				case "IsDirty":
					UpdateDisplayName();
					NotifyOfPropertyChange(() => IsDirty);
					break;
			}

		}

		/// <summary>
		/// Save the record
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IResult> Save()
		{

			RecordModel.Save();

			yield break;

		}

	}
}
