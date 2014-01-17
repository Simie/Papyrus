/*
 * Copyright © 2014 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System.Windows.Controls;
using Papyrus.Studio.Framework;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace Papyrus.Studio.Modules.GenericRecordEditor.Views
{

	/// <summary>
	/// Interaction logic for GenericRecordView.xaml
	/// </summary>
	public partial class GenericRecordView : UserControl
	{
		public GenericRecordView()
		{

			InitializeComponent();

			//PropGrid.PropertyItemFactory = new PapyrusPropertyItemFactory();
			PropGrid.PropertyControlFactory = PapyrusPropertyControlFactory.GetControlFactory();

		}
	}
}
