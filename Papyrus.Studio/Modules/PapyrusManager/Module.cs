﻿/*
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
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Modules.MainMenu.Models;
using Papyrus.Studio.Framework.Results;
using Papyrus.Studio.Properties;

namespace Papyrus.Studio.Modules.PapyrusManager
{
	[Export(typeof(IModule))]
	class Module : ModuleBase
	{

#pragma warning disable 0649
		[Import(typeof(IPapyrusManager))] private IPapyrusManager _papyrusManager;
#pragma warning restore 0649

		public override void Initialize()
		{

			var menuItem = Shell.MainMenu.FirstOrDefault(p => p.Name == "File") as MenuItem;

			if (menuItem != null)
			{

				menuItem.Children.Insert(0, new MenuItemSeparator()); 
				
				var prevSession = new CheckableMenuItem("Load Previous Session On Start", CheckLoadSessionOnStart);
				prevSession.IsChecked = Settings.Default.LoadPreviousSession;
				menuItem.Children.Insert(0, prevSession);

				menuItem.Children.Insert(0, new MenuItem("Save Plugin", _papyrusManager.SaveActivePlugin).WithGlobalShortcut(ModifierKeys.Control | ModifierKeys.Shift, Key.S));
				menuItem.Children.Insert(0, new MenuItem("Select Data Directory", _papyrusManager.SelectDataDirectory));
				menuItem.Children.Insert(0, new MenuItem("Select Data Files", _papyrusManager.SelectDataFiles));
				

			}

			menuItem = Shell.MainMenu.FirstOrDefault(p => p.Name == "View") as MenuItem;

			if (menuItem != null) {

				menuItem.Children.Insert(0, new MenuItem("View Active Plugin Summary", _papyrusManager.ViewActivePluginSummary));

			}

			/*Exception error = null;

			if (Settings.Default.LoadPreviousSession && !string.IsNullOrWhiteSpace(Settings.Default.PreviousActivePlugin)) {

				try {

					(_papyrusManager as ViewModels.PapyrusManagerViewModel).LoadPlugin(Settings.Default.PreviousActivePlugin,
											   new List<string>(Settings.Default.SelectedMasters.OfType<string>()));

				}
				catch (Exception e) {
					error = e;
					MessageBox.Show(e.Message, "Error Loading Previous Session", MessageBoxButton.OK);
				}

			}

			if (error != null) {
				//Coroutine.BeginExecute(ShowExt.Exception(error));
			}*/

			//Shell.MainMenu.First(p => p.Name == "File").Add(new MenuItem("Select Data Files", _papyrusManager.SelectDataFiles));
		}

		private IEnumerable<IResult> CheckLoadSessionOnStart(bool b)
		{

			Settings.Default.LoadPreviousSession = b;
			Settings.Default.Save();
			yield break;

		}
	}
}
