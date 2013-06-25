﻿/*
 * Copyright © 2013 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.MainMenu.Models;
using Papyrus.Studio.Framework;
using Papyrus.Studio.Framework.ComponentModel;
using Papyrus.Studio.Modules.PapyrusManager;

namespace Papyrus.Studio.Modules.Startup
{
	[Export(typeof (IModule))]
	public class StartupModule : ModuleBase
	{
		private const string Title = "Papyrus Studio";

#pragma warning disable 0649
		[Import(typeof(IShell))]
		private IShell _shell;

		[Import(typeof (IPapyrusManager))]
		private IPapyrusManager _papyrusManager;
#pragma warning restore 0649

		private Papyrus.Core.PluginComposer _pluginComposer;

		private MenuItem _saveMenuItem;
		private MenuItem _saveAllMenuItem;

		private IScreen _previousActiveItem;

		public override void Initialize()
		{

			TypeDescriptor.AddProvider(new RecordTypeProvider(), typeof(Core.Record));

			_shell.Title = Title;

			if (_papyrusManager.PluginComposer != null)
			{
				_pluginComposer = _papyrusManager.PluginComposer;
				WireDatabase();
			}

			_papyrusManager.PropertyChanged += PapyrusManagerOnPropertyChanged;

			_saveMenuItem = new MenuItem("Save", SaveExecute, SaveCanExecute).WithGlobalShortcut(ModifierKeys.Control, Key.S) as MenuItem;
			_saveAllMenuItem =
				new MenuItem("Save All", SaveAllExecute).WithGlobalShortcut(ModifierKeys.Control | ModifierKeys.Shift, Key.S) as
				MenuItem;
			
			var fileMenu = _shell.MainMenu.First(p => p.Name == "File");

			//fileMenu.Children.Insert(0, _saveMenuItem);
			var index = fileMenu.Children.IndexOf(fileMenu.Children.First(p => p is MenuItemSeparator));

			fileMenu.Children.Insert(index, _saveAllMenuItem);
			fileMenu.Children.Insert(index, _saveMenuItem);
			fileMenu.Children.Insert(index, new MenuItemSeparator());

			var helpMenu = _shell.MainMenu.FirstOrDefault(p => p.Name == "Help");

			if (helpMenu == null) {
				helpMenu = new MenuItem("Help", HelpMenuExecute);
				_shell.MainMenu.Add(helpMenu);
			}

			helpMenu.Add(new MenuItem("About"));


			if (_shell is INotifyPropertyChanged) {
				(_shell as INotifyPropertyChanged).PropertyChanged += ShellPropertyChanged;
			}

		}

		private IEnumerable<IResult> HelpMenuExecute()
		{

			/*var diag = new PropertyTools.Wpf.AboutDialog(Application.Current.MainWindow);
			
			diag.Show();*/

			yield break;

		}

		private void UpdateSaveButton()
		{

			if (_shell.ActiveItem != null)
			{
				_saveMenuItem.Text = string.Format("Save [{0}]", _shell.ActiveItem.DisplayName);

			}
			else
			{
				_saveMenuItem.Text = "Save";
			}

		}

		private void ShellPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{

			if (propertyChangedEventArgs.PropertyName != "ActiveItem")
				return;

			if(_previousActiveItem != null)
				_previousActiveItem.PropertyChanged -= ActiveItemPropertyChanged;

			UpdateSaveButton();

			if(_shell.ActiveItem != null)
				_shell.ActiveItem.PropertyChanged += ActiveItemPropertyChanged;

			_previousActiveItem = _shell.ActiveItem;

			_saveMenuItem.NotifyOfPropertyChange(() => _saveMenuItem.CanExecute);

		}

		private void ActiveItemPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			
			UpdateSaveButton();

		}

		private IEnumerable<IResult> SaveAllExecute()
		{

			

			MessageBox.Show("SaveAll not implemented");
			yield break;
			//throw new NotImplementedException();

		}

		private IEnumerable<IResult> SaveExecute()
		{


			if (_shell.ActiveItem is ISaveAware)
				yield return new SequentialResult((_shell.ActiveItem as ISaveAware).Save().GetEnumerator());


		}

		private bool SaveCanExecute()
		{

			return _shell.ActiveItem is ISaveAware;

		}


		private void WireDatabase()
		{

			//_pluginComposer.PropertyChanged += RecordDatabaseOnPropertyChanged;
			UpdateTitle();

		}

		void UpdateTitle()
		{



			if (_pluginComposer == null) {
				Shell.Title = Title;
				return;
			}

			Shell.Title = _pluginComposer.Plugin.Name;

			//if (_pluginComposer.NeedsSaving)
			//	Shell.Title += "*";

			Shell.Title += " - " + Title;


		}

		void UnwireDatabase()
		{
			if (_pluginComposer == null)
				return;

			//_pluginComposer.PropertyChanged += RecordDatabaseOnPropertyChanged;

		}

		private void RecordDatabaseOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			
			UpdateTitle();

		}

		private void PapyrusManagerOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{

			if (propertyChangedEventArgs.PropertyName == "RecordDatabase") {
				
				UnwireDatabase();
				_pluginComposer = _papyrusManager.PluginComposer;

				if(_pluginComposer != null)
					WireDatabase();

			}

		}
	}
}
