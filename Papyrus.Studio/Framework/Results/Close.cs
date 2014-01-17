﻿/*
 * Copyright © 2014 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gemini.Framework;

namespace Papyrus.Studio.Framework.Results
{

	public static class Close
	{

		public static CloseDocumentResult Document(IDocument document)
		{
			return new CloseDocumentResult(document);
		}

	}

}
