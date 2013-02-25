﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Papyrus.Studio.Framework.Services;

namespace Papyrus.Studio.Framework.Results
{
	public class ShowExceptionResult : IResult
	{
		private Exception _exception;

#pragma warning disable 0649
		[Import] private IExceptionHandler _exceptionHandler;
#pragma warning restore 0649

		public ShowExceptionResult(Exception e)
		{
			_exception = e;
		}

		public event EventHandler<ResultCompletionEventArgs> Completed;

		public void Execute(ActionExecutionContext context)
		{

			if (_exceptionHandler == null || _exception == null) {
				OnCompleted(null);
				return;
			}

			_exceptionHandler.HandleException(_exception, () => OnCompleted(null));

		}

		protected void OnCompleted(Exception exception)
		{

			if (Completed != null)
				Completed(this, new ResultCompletionEventArgs { Error = exception });

		}

	}
}
