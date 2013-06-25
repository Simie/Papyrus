﻿/*
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
using Papyrus.Studio.Framework.Services;

namespace Papyrus.Studio.Modules.PapyrusManager
{

	public interface IPapyrusManager : INotifyPropertyChangedEx
	{

		string DataPath { get; }

		event EventHandler RecordDatabaseChanged;
		
		/// <summary>
		/// Opens the data file selection screen
		/// </summary>
		/// <returns></returns>
		IEnumerable<IResult> SelectDataFiles();

		/// <summary>
		/// The active record database, or null if no database is loaded.
		/// </summary>
		Papyrus.Core.PluginComposer PluginComposer { get; }

		/// <summary>
		/// Opens the given record in the default editor.
		/// </summary>
		/// <param name="record"></param>
		IEnumerable<IResult> OpenRecord(Papyrus.Core.Record record);

		/// <summary>
		/// Opens the given record in the default editor.
		/// </summary>
		/// <param name="recordType">Record type</param>
		/// <param name="record">Record key</param>
		IEnumerable<IResult> OpenRecord(Type recordType, Papyrus.Core.RecordKey record);

		/// <summary>
		/// Open the given record in the provided editor, or pass null to display a list to choose from.
		/// </summary>
		/// <param name="record">Record to open</param>
		/// <param name="provider">EditorProvider to open the record in, or null to display a list.</param>
		/// <returns></returns>
		IEnumerable<IResult> OpenRecordWith(Papyrus.Core.Record record, IRecordEditorProvider provider = null);
			
		/// <summary>
		/// Saves the plugin being edited.
		/// </summary>
		/// <returns></returns>
		IEnumerable<IResult> SaveActivePlugin();

		//void LoadPlugin(string activePlugin, List<string> masters);

		/// <summary>
		/// Displays the current plugin summary in a message box
		/// </summary>
		/// <returns></returns>
		IEnumerable<IResult> ViewActivePluginSummary();

		/// <summary>
		/// Begins the process of choosing a new data directory
		/// </summary>
		/// <returns></returns>
		IEnumerable<IResult> SelectDataDirectory();

	}

}
