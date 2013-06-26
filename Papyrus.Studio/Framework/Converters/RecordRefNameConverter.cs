using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Caliburn.Micro;
using Papyrus.Core;
using Papyrus.Studio.Modules.PapyrusManager;

namespace Papyrus.Studio.Framework.Converters
{

	/// <summary>
	/// Converts a RecordRef to the EditorID of that record
	/// </summary>
	public class RecordRefNameConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			
			var recordRef = value as IRecordRef;

			if (recordRef == null) {
				return value;
			}

			if (recordRef.Key == RecordKey.Identity)
				return "Empty";


			try {

				var papyrusManager = IoC.Get<IPapyrusManager>();

				var record = papyrusManager.PluginComposer.GetRecord(recordRef.Type, recordRef.Key);

				return string.Format("{0} ({1})", record.EditorID, record.Key);

			} catch {

				return "ERROR";

			}

		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

	}
}
