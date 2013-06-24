using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Papyrus.Core;

namespace Papyrus.Studio.Modules.PapyrusManager.ViewModels
{

	public class PluginViewModel : PropertyChangedBase
	{

		public Plugin Plugin { get; private set; }

		public string Name { get { return Plugin.Name; } }
		public string Author { get { return Plugin.Author; } }
		public string Description { get { return Plugin.Description; } }

		private bool _isEnabled;

		/// <summary>
		/// Is this plugin enabled for loading
		/// </summary>
		public bool IsEnabled
		{
			get { return _isEnabled; }
			set
			{
				_isEnabled = value;
				NotifyOfPropertyChange(() => IsEnabled);
			}
		}

		private bool _isActive;

		/// <summary>
		/// Is this plugin set as the active edit plugin
		/// </summary>
		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				_isActive = value;
				NotifyOfPropertyChange(() => IsActive);
			}
		}

		public PluginViewModel(Plugin p)
		{
			Plugin = p;
		}

	}

}
