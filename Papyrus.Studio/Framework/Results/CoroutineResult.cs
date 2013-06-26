/*
 * Copyright © 2013 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace Papyrus.Studio.Framework.Results
{

	public static class EnumeratorExtensions
	{

		public static IResult ToResult(this IEnumerable<IResult> coroutine)
		{
			return new CoroutineResult(coroutine.GetEnumerator());
		}

	}

	public class CoroutineResult : IResult
	{

		private readonly IEnumerator<IResult> _coroutine; 

		public CoroutineResult(IEnumerator<IResult> coroutine)
		{
			_coroutine = coroutine;
		}

		public void Execute(ActionExecutionContext context)
		{
			Coroutine.BeginExecute(_coroutine, context, Completed);
		}

		public event EventHandler<ResultCompletionEventArgs> Completed;

	}

}
