/*
 * Copyright © 2014 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Papyrus.Core;
using Papyrus.Core.Util;
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

			_plugins = new BindableCollection<PluginViewModel>(PluginUtil.SortPluginList(plugins.ToList(), p => p.Plugin));

			foreach (var plugin in _plugins) {
				plugin.PropertyChanged += PluginOnPropertyChanged;
			}

			DataDirectory = dataDirectory;
			//SelectedPlugin = plugins.FirstOrDefault();

			DisplayName = "Papyrus Setup";

		}

		private void PluginOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			
			NotifyOfPropertyChange(() => CanAccept);

		}

		public bool CanAccept
		{
			get
			{

				var activePlugin = Plugin.FirstOrDefault(p => p.IsActive);

				return activePlugin != null && !Plugin.Where(p => p.IsEnabled).Any(p => p.Parents.Contains(activePlugin.Name));

			}
		}

		public void Accept()
		{
			IsCancelled = false;

			foreach (var pluginViewModel in _plugins) {
				pluginViewModel.PropertyChanged -= PluginOnPropertyChanged;
			}

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

			// Disable any plugins that depend on this plugin
			foreach (var plugin in _plugins) {
				if (plugin.Parents.Contains(SelectedPlugin.Name))
					plugin.IsEnabled = false;
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

			newPlugin.PropertyChanged += PluginOnPropertyChanged;

			ActivateSelectedPlugin();


			yield break;
		}

	}

}
