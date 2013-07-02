/*
 * Copyright © 2013 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Gemini.Framework.Results;
using Gemini.Framework.Services;
using Papyrus.Core;
using Papyrus.Studio.Framework;
using Papyrus.Studio.Framework.Results;
using Papyrus.Studio.Framework.Services;
using Papyrus.Studio.Properties;

namespace Papyrus.Studio.Modules.PapyrusManager.ViewModels
{

	[Export(typeof (IPapyrusManager))]
	public class PapyrusManagerViewModel : Screen, IPapyrusManager
	{

		public string DataPath { get { return Settings.Default.DataPath; } }

		public event EventHandler RecordDatabaseChanged = delegate { };

		[Import(typeof (IShell))]
		[SuppressMessage("Microsoft.Performance", "CA1823")]
		private IShell _shell;

		private Papyrus.Core.PluginComposer _pluginComposer;

		public Papyrus.Core.PluginComposer PluginComposer
		{
			get { return _pluginComposer; }
			private set
			{
				_pluginComposer = value;
				NotifyOfPropertyChange(() => PluginComposer);
			}
		}

		private List<PluginViewModel> _activeMasters;
		private PluginViewModel _activePlugin;

		public PapyrusManagerViewModel()
		{
		}

		private void Init()
		{


		}

		public void LoadPlugin(Plugin activePlugin, List<Plugin> masters)
		{

			Init();

			// Ensure master list doesn't contain active plugin.
			masters.Remove(activePlugin);

			PluginComposer = PluginComposer.EditPlugin(activePlugin, masters);
			RecordDatabaseChanged(this, EventArgs.Empty);

			_activeMasters = masters.Select(p =>  new PluginViewModel(p)).ToList();
			_activePlugin = new PluginViewModel(activePlugin);

			Settings.Default.PreviousActivePlugin = activePlugin.Name;
			Settings.Default.SelectedMasters = new StringCollection();
			Settings.Default.SelectedMasters.AddRange(masters.Select(p => p.Name).ToArray());
			Settings.Default.Save();

		}

		private List<IRecordEditorProvider> EditorProvidersForRecord(Record record)
		{

			var editorProviders =
				IoC.GetAllInstances(typeof (IRecordEditorProvider)).Cast<IRecordEditorProvider>().Where(p => p.Handles(record));

			var recordType = record.GetType();
			return editorProviders.OrderByDescending(p => p.PrimaryType == recordType).ToList();

		}

		public IEnumerable<IResult> CloseAllEditors()
		{

			var openDocuments = GetOpenRecordEditors();

			foreach (var document in openDocuments) {
				document.TryClose();
			}

			if (GetOpenRecordEditors().Count > 0) {
				MessageBox.Show("Could not close all editors.");
				yield break;
			}

		} 

		public ICollection<IRecordDocument> GetOpenRecordEditors()
		{
			return _shell.Documents.Where(p => p is IRecordDocument).Cast<IRecordDocument>().ToList();
		}  

		public IEnumerable<IResult> SelectDataDirectory()
		{

			yield return new SequentialResult(CloseAllEditors().GetEnumerator());

			var currentPath = Settings.Default.DataPath ?? Environment.CurrentDirectory;

			var folder = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();

			folder.SelectedPath = currentPath;
			folder.UseDescriptionForTitle = true;
			folder.Description = Resources.Select_Data_Directory;
			var success = folder.ShowDialog().GetValueOrDefault();

			if (!success)
				yield break;

			Settings.Default.DataPath = folder.SelectedPath;
			Settings.Default.Save();

		}


		public IEnumerable<IResult> SelectDataFiles()
		{

			Init();

			yield return new SequentialResult(CloseAllEditors().GetEnumerator());

			if (PluginComposer != null && PluginComposer.NeedSaving) {

				var result = SaveUtil.ShowSaveDialog("Plugin");

				if(result == SaveUtil.SaveDialogResult.Cancel)
					yield break;

				if (result == SaveUtil.SaveDialogResult.Save)
					yield return SaveActivePlugin().ToResult();

			}

			if(string.IsNullOrWhiteSpace(DataPath))
				yield return new SequentialResult(SelectDataDirectory().GetEnumerator());

			var setup = new PapyrusSetupViewModel(PluginLoader.ScanDirectory(DataPath).Select(p => new PluginViewModel(p)), Settings.Default.DataPath);

			if (_activeMasters != null) {

				foreach (var plugin in setup.Plugin) {

					plugin.IsEnabled = _activeMasters.Contains(plugin);

					if (_activePlugin.Name == plugin.Name)
						plugin.IsActive = plugin.IsEnabled = true;

				}
			}

			yield return ShowExt.Modal(setup);

			List<Plugin> selectedMasters = setup.Plugin.Where(p => p.IsEnabled).Select(p => p.Plugin).ToList();
			Plugin activePlugin = setup.Plugin.Single(p => p.IsActive).Plugin;

			Exception error = null;

			try {

				LoadPlugin(activePlugin, selectedMasters);

			}
			catch (Exception e) {

				error = e;

			}

			if (error != null) {
				yield return ShowExt.Exception(error);
			}

		}


		public override void CanClose(Action<bool> callback)
		{

			if (_pluginComposer == null/* || !_pluginComposer.NeedsSaving*/) {
				callback(true);
				return;
			}

			var result = SaveUtil.ShowSaveDialog(string.Format("Plugin {0}", _pluginComposer.Plugin.Name));

			switch (result) {
				case SaveUtil.SaveDialogResult.Cancel:
					callback(false);
					return;
				case SaveUtil.SaveDialogResult.Save:
					Coroutine.BeginExecute(SaveActivePlugin().GetEnumerator());
					break;
			}

			callback(true);
			return;

		}

		public IEnumerable<IResult> SaveActivePlugin()
		{

			if (PluginComposer != null) {

				PluginComposer.SavePlugin(DataPath);

			}

			yield break;

		}

		public IEnumerable<IResult> OpenRecord(Record record)
		{

			var existingEditor = FindExistingEditor(record);

			if (existingEditor != null) {
				yield return Show.Document(existingEditor);
				yield break;
			}

			var editorProvider = EditorProvidersForRecord(record).FirstOrDefault();

			yield return new SequentialResult(OpenRecordWith(record, editorProvider).GetEnumerator());

		}

		private IRecordDocument FindExistingEditor(Record record)
		{
			var recordType = record.GetType();
			var existingEditor =
				GetOpenRecordEditors().Where(p => p.Record.GetType() == recordType).FirstOrDefault(p => p.Record.Key == record.Key);
			return existingEditor;
		}

		public IEnumerable<IResult> OpenRecord(Type recordType, RecordKey record)
		{

			var r = _pluginComposer.GetRecord(recordType, record);
			yield return OpenRecord(r).ToResult();

		}

		/// <summary>
		/// Open the given record in the provided editor, or pass null to display a list to choose from.
		/// </summary>
		/// <param name="record">Record to open</param>
		/// <param name="provider">EditorProvider to open the record in, or null to display a list.</param>
		/// <returns></returns>
		public IEnumerable<IResult> OpenRecordWith(Record record, IRecordEditorProvider provider)
		{

			if (provider == null) {

				var providers = EditorProvidersForRecord(record);
				//provider = providers.LastOrDefault();

				var editorSelect = new EditorSelectViewModel(providers);
				yield return ShowExt.Modal(editorSelect);

				provider = editorSelect.SelectedEditor;

			}

			var existingEditor = FindExistingEditor(record);

			if (existingEditor != null) {

				if (provider.IsInstanceOf(existingEditor)) {
					yield return Show.Document(existingEditor);
					yield break;
				}

				var result = MessageBox.Show(
					"Record is already open in a different editor. A record can only be open in a single editor at one time. Close the existing editor?",
					"Existing Editor", MessageBoxButton.OKCancel, MessageBoxImage.Question);

				if(result == MessageBoxResult.Cancel)
					yield break;

				var closeResult = Close.Document(existingEditor);
				yield return closeResult;

				if(closeResult.Cancelled)
					yield break;

			}

			IRecordDocument document = null;
			Exception error = null;


			try {
				document = provider.Create(record);
			} catch (Exception e) {
				error = e;
			}

			if (error != null) {
				yield return ShowExt.Exception(error);
				MessageBox.Show("Error Opening Record", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				yield break;
			}

			yield return Show.Document(document);

		} 

		public IEnumerable<IResult> ViewActivePluginSummary()
		{

			if (PluginComposer == null)
				yield break;

			//var summary = PluginComposer.ActivePluginSummery();

			//var longMsg = new LongMessageBox();
			//longMsg.textBox.Text = summary;
			//longMsg.ShowDialog();

		}


	}

}
