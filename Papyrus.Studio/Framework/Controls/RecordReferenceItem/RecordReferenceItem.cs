/*
 * Copyright © 2014 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using Papyrus.Core;
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
			typeof(IRecordRef),
			typeof(RecordReferenceItem),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, RecordReferenceChangedCallback));

		private IPapyrusManager _papyrusManager;

		/// <summary>
		/// Gets or sets the data pointer.
		/// </summary>
		public IRecordRef RecordReference
		{
			get
			{
				return (IRecordRef)this.GetValue(RecordReferenceProperty);
			}

			set
			{
				this.SetValue(RecordReferenceProperty, value);
			}
		}

		public event EventHandler RecordReferenceChanged;

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

		public ICommand ClearCommand { get; set; }

		static RecordReferenceItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(
				typeof(RecordReferenceItem), new FrameworkPropertyMetadata(typeof(RecordReferenceItem)));
		}

		public RecordReferenceItem()
		{

			BrowseCommand = new DelegateCommand(Browse);
			OpenCommand = new DelegateCommand(Open, () => RecordReference != null && RecordReference.Key != RecordKey.Identity);
			ClearCommand = new DelegateCommand(Clear);

			if(!DesignerProperties.GetIsInDesignMode(this))
				_papyrusManager = IoC.Get<IPapyrusManager>();

		}

		private void Browse()
		{

			RecordReference = RecordPicker.PickRecord(RecordReference);

		}

		private void Open()
		{

			if (this.RecordReference != null && this.RecordReference.Key != RecordKey.Identity) {

				Coroutine.BeginExecute(_papyrusManager.OpenRecord(this.RecordReference.ValueType, this.RecordReference.Key).GetEnumerator());

			}

		}

		private void Clear()
		{
			RecordReference = (IRecordRef)Activator.CreateInstance(typeof (RecordRef<>).MakeGenericType(RecordReference.Type),
			                                           RecordKey.Identity, null);
		}

		protected void OnRecordReferenceChanged(IRecordRef oldReference, IRecordRef newReference)
		{

			if (RecordReferenceChanged != null)
				RecordReferenceChanged(this, EventArgs.Empty);

		}

		private static void RecordReferenceChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
		{
			((RecordReferenceItem)dependencyObject).OnRecordReferenceChanged(args.OldValue as IRecordRef,
																			  args.NewValue as IRecordRef);
		}

	}

}
