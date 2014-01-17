/*
 * Copyright © 2014 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using Gemini;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace Papyrus.Studio
{
	public class EditorBootstrapper : AppBootstrapper
	{

		public static readonly List<string> PapyrusModules = new List<string>();

		public List<string> Modules { get { return PapyrusModules; } } 

		protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
		{
			
			base.OnStartup(sender, e);

			var shell = IoC.Get<IShell>();

			var conductor = shell as Conductor<IDocument>.Collection.OneActive;

			if (conductor == null) {
				throw new InvalidOperationException("Could not locate Conductor. Has Gemini changed implementation?");
			}

			conductor.CloseStrategy = new PapyrusCloseStrategy();

		}

	}
}
