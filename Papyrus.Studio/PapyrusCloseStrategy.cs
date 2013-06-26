/*
 * Copyright © 2013 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Gemini.Framework;
using Papyrus.Studio.Framework.Results;
using Papyrus.Studio.Modules.PapyrusManager;
using Gemini.Framework.Services;

namespace Papyrus.Studio
{

	/// <summary>
	/// Heck fest to query papyrus manager before closing
	/// </summary>
	class PapyrusCloseStrategy : ICloseStrategy<IDocument>
	{

		private readonly DefaultCloseStrategy<IDocument> _internalStrategy;

		public PapyrusCloseStrategy()
		{
			_internalStrategy = new DefaultCloseStrategy<IDocument>(false);
		}

		IEnumerable<IResult> ReportError(Exception e)
		{
			yield return ShowExt.Exception(e);
		} 

		public void Execute(IEnumerable<IDocument> toClose, Action<bool, IEnumerable<IDocument>> callback)
		{


			var conductor = IoC.Get<IShell>() as Caliburn.Micro.Conductor<IDocument>.Collection.OneActive;

			if (conductor == null) {
				Coroutine.BeginExecute(ReportError(new InvalidOperationException("Could not locate Conductor. Has Gemini changed implementation?")).GetEnumerator());
				return;
			}

			// If it's closing only a few items, pass it on to the default strategy
			if (conductor.Items != toClose) {
				_internalStrategy.Execute(toClose, callback);
				return;
			}

			Framework.SaveUtil.BeginSaveOperation();

			Exception error = null;

			try {

				// If it's a full shutdown, we're going to invervene
				_internalStrategy.Execute(toClose, (editorsCanClose, screens) => {

					Framework.SaveUtil.EndSaveOperation();

					// If an editor has already cancelled shutdown, we don't need to stop it.
					if (!editorsCanClose) {
						callback(editorsCanClose, screens);
						return;
					}



					var p = IoC.Get<IPapyrusManager>() as
					        Modules.PapyrusManager.ViewModels.PapyrusManagerViewModel;

					// Check if papyrus has a problem closing
					p.CanClose(papyrusCanClose => callback(papyrusCanClose, screens));

				});

			} catch (Exception e) {
				error = e;
				Framework.SaveUtil.EndSaveOperation();
			}

			if (error != null) {
				Coroutine.BeginExecute(ReportError(error).GetEnumerator());
			}


		}

	}
}
