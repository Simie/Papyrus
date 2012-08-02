﻿/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Papyrus.Design
{
	public static class DataPointerUtils
	{
		/// <summary>
		/// Resolves a given data pointer using the given database.
		/// </summary>
		/// <param name="pointer"></param>
		/// <param name="database"></param>
		/// <param name="throwOnError">True to throw an exception if resolving the pointer throws an exception.</param>
		public static void ResolveDataPointer(DataPointer pointer, RecordDatabase database, bool throwOnError = false)
		{

			try {

				pointer.ResolvePointer(database);

			} catch (Exception) {
				if (throwOnError)
					throw;
			}

		}

	}
}
