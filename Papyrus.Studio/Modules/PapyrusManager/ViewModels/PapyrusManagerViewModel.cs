/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Results;
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

		/// <summary>
		/// True if papyrus has already been initialised (will need a restart if true and modules change)
		/// </summary>
		private static bool _papyrusInit;

		//[Import(typeof (IShell))] private IShell _shell;

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


		private List<IRecordDocument> _recordEditors = new List<IRecordDocument>(); 

		public PapyrusManagerViewModel()
		{
		}

		private void Init()
		{


			if (!_papyrusInit)
			{



				Exception error = null;

				try {

					//Papyrus.RecordDatabase.Initialize(EditorBootstrapper.PapyrusModules);

					_papyrusInit = true;

					bool yesToAll = false;

					/*Papyrus.Config.ReferenceErrorCallback = pointer =>
					{
						if (yesToAll)
							return true;

						var handler = new PapyrusErrorViewModel();
						return handler.HandleException(pointer, ref yesToAll);
					};*/

				} catch (Exception e) {
					error = e;
				}

				if (error != null) {
					ShowExt.Exception(error);
				}

			}


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

			var openDocumentsCopy = new List<IDocument>(_recordEditors);

			foreach (var document in openDocumentsCopy) {
				document.TryClose();
			}

			if (_recordEditors.Count > 0) {
				MessageBox.Show("Could not close all editors.");
				yield break;
			}

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
				PluginLoader.SavePlugin(PluginComposer.Plugin, DataPath);
			}

			yield break;

		}

		public IEnumerable<IResult> OpenRecord(Record record)
		{

			var existingEditor = _recordEditors.FirstOrDefault(p => p.Record.Equals(record));

			if (existingEditor != null) {
				yield return Show.Document(existingEditor);
				yield break;
			}

			var editorProvider = EditorProvidersForRecord(record).FirstOrDefault();

			yield return new SequentialResult(OpenRecordWith(record, editorProvider).GetEnumerator());

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

			var existingEditor = _recordEditors.FirstOrDefault(p => p.Record == record);

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

			_recordEditors.Add(document);

			yield return Show.Document(document);

			_recordEditors.Remove(document);

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
