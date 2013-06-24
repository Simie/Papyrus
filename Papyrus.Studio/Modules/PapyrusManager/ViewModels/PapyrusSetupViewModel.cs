/*
 * Copyright © 2013 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Papyrus.Core;
using Papyrus.Studio.Framework.Results;

namespace Papyrus.Studio.Modules.PapyrusManager.ViewModels
{

	public class PapyrusSetupViewModel : Caliburn.Micro.Screen
	{

		private bool _isCancelled;
		/// <summary>
		/// True if this setup view was cancelled.
		/// </summary>
		public bool IsCancelled
		{
			get { return _isCancelled; }
			set { _isCancelled = value; NotifyOfPropertyChange(() => IsCancelled); }
		}

		private BindableCollection<PluginViewModel> _plugins;
		public IObservableCollection<PluginViewModel> Plugin
		{
			get { return _plugins; }
		}

		private PluginViewModel _selectedPlugin;
		public PluginViewModel SelectedPlugin
		{
			get { return _selectedPlugin; }
			set
			{
				_selectedPlugin = value; 
				NotifyOfPropertyChange(() => SelectedPlugin);
				NotifyOfPropertyChange(() => CanConvertPlugin);
			}
		}

		public bool CanConvertPlugin { get { return SelectedPlugin != null; } }

		public string DataDirectory { get; private set; }

		public PapyrusSetupViewModel(IEnumerable<PluginViewModel> plugins, string dataDirectory)
		{

			_plugins = new BindableCollection<PluginViewModel>(plugins);
			DataDirectory = dataDirectory;
			//SelectedPlugin = plugins.FirstOrDefault();

			DisplayName = "Papyrus Setup";

		}

		public bool CanAccept
		{
			get { return Plugin.Any(p => p.IsActive); }
		}

		public void Accept()
		{
			IsCancelled = false;
		}

		public void Cancel()
		{
			IsCancelled = true;
		}

		public void ActivateSelectedPlugin()
		{

			if (SelectedPlugin == null)
				return;

			foreach (var pluginInfo in Plugin) {
				pluginInfo.IsActive = false;
			}

			SelectedPlugin.IsEnabled = true;

			// Enable any dependencies of this plugin
			foreach (var dependency in SelectedPlugin.Plugin.Parents) {
				Plugin.Where(p =>p.Name == dependency).ToList().ForEach(p => p.IsEnabled = true);
			}

			SelectedPlugin.IsActive = true;
			NotifyOfPropertyChange(() => CanAccept);


		}

		public IEnumerable<IResult> NewPlugin()
		{
			
			var newPluginWizard = new NewPluginViewModel();
			IoC.BuildUp(newPluginWizard);
			yield return ShowExt.Modal(newPluginWizard);

			var plugin = PluginComposer.CreateBlank(newPluginWizard.PluginName);
			plugin.Plugin.Author = newPluginWizard.PluginAuthor;
			plugin.Plugin.Description = newPluginWizard.PluginDescription;

			PluginLoader.SavePlugin(plugin.Plugin, DataDirectory);

			var newPlugin = new PluginViewModel(plugin.Plugin);
			_plugins.Add(newPlugin);

			SelectedPlugin = newPlugin;

			ActivateSelectedPlugin();

			/*var plugin = Papyrus.PluginUtilities.CreateNewPlugin(newPlugin.PluginName, newPlugin.PluginDirectory, newPlugin.PluginType.Format);

			plugin.Description = newPlugin.PluginDescription;
			plugin.Author = newPlugin.PluginAuthor;
			
			Papyrus.PluginUtilities.ApplyPluginInfo(plugin);

			plugin.IsValid = true;

			Plugin.Add(plugin);
			SelectedPlugin = plugin;

			ActivateSelectedPlugin();*/


			yield break;
		}

	}

}
