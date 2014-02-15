using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Modules.MainMenu.Models;
using Papyrus.Studio.Modules.RecordTable.ViewModels;

namespace Papyrus.Studio.Modules.RecordTable
{
	[Export(typeof(IModule))]
	class Module : ModuleBase
	{

		public override void Initialize()
		{
			base.Initialize();

			MainMenu.All.First(p => p.Name == "View").Add(new MenuItem("Record Tables", OpenRecordTables));

		}

		private IEnumerable<IResult> OpenRecordTables()
		{
			yield return Show.Document<RecordTableViewModel>();
		}

	}
}
