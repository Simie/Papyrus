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
