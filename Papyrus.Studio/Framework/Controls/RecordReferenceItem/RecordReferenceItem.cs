﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using Papyrus.Studio.Modules.PapyrusManager;
using PropertyTools.Wpf;

namespace Papyrus.Studio.Framework.Controls
{

	public class RecordReferenceItem : Control
	{

		/// <summary>
		/// The directory property.
		/// </summary>
		public static readonly DependencyProperty RecordReferenceProperty = DependencyProperty.Register(
			"RecordReference",
			typeof(RecordReference),
			typeof(RecordReferenceItem),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		/// <summary>
		/// Gets or sets the data pointer.
		/// </summary>
		public RecordReference RecordReference
		{
			get
			{
				return (RecordReference)this.GetValue(RecordReferenceProperty);
			}

			set
			{
				this.SetValue(RecordReferenceProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the browse command.
		/// </summary>
		/// <value> The browse command. </value>
		public ICommand BrowseCommand { get; set; }
	
		/// <summary>
		/// Gets or sets the open command.
		/// </summary>
		/// <value> The browse command. </value>
		public ICommand OpenCommand { get; set; }

		static RecordReferenceItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(
				typeof(RecordReferenceItem), new FrameworkPropertyMetadata(typeof(RecordReferenceItem)));
		}

		public RecordReferenceItem()
		{
			BrowseCommand = new DelegateCommand(Browse);
			OpenCommand = new DelegateCommand(Open, () => RecordReference != null && RecordReference.IsValid);
		}

		private void Browse()
		{

			RecordReference = Papyrus.Design.Controls.RecordPicker.PickRecord(RecordReference);

		}

		private void Open()
		{

			if (this.RecordReference != null && this.RecordReference.IsValid) {

				var papyrusManager = IoC.Get<IPapyrusManager>();
				Coroutine.BeginExecute(papyrusManager.OpenRecord(this.RecordReference.Record).GetEnumerator());

			}

		}

	}

}
