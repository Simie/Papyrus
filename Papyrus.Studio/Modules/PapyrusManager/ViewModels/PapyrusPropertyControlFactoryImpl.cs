using System.Collections;
using System.ComponentModel.Composition;
using System.Windows;
using Papyrus.Core;
using Papyrus.Studio.Framework.Controls;
using Papyrus.Studio.Framework.Services;
using PropertyTools.Wpf;

namespace Papyrus.Studio.Modules.PapyrusManager.ViewModels
{
	[Export(typeof(IPropertyControlProvider))]
	public class PapyrusPropertyControlProvider : IPropertyControlProvider
	{

		public FrameworkElement CreateControl(PropertyItem property, PropertyControlFactoryOptions options)
		{

			if (typeof(IRecordRef).IsAssignableFrom(property.ActualPropertyType)) {

				return CreateRecordReferenceControl(property);

			}

			if (typeof(IRecordRefCollection).IsAssignableFrom(property.ActualPropertyType)) {

				return CreateRecordReferenceListControl(property);

			}

			if (property.ActualPropertyType.IsGenericType &&
				property.ActualPropertyType.GetGenericTypeDefinition() == typeof(ReadOnlyCollection<>)) {

				return CreateCollectionControl(property);

			}

			return null;

		}

		public FrameworkElement CreateRecordReferenceControl(PropertyItem item)
		{

			var c = new RecordReferenceItem();
			c.SetBinding(RecordReferenceItem.RecordReferenceProperty, item.CreateBinding());
			return c;

		}

		public FrameworkElement CreateRecordReferenceListControl(PropertyItem item)
		{

			var c = new RecordList();
			c.SetBinding(RecordList.SourceListProperty, item.CreateBinding());
			return c;

		}

		public FrameworkElement CreateCollectionControl(PropertyItem item)
		{

			var c = new CollectionEditor();
			c.SetBinding(CollectionEditor.ItemsSourceProperty, item.CreateBinding());
			return c;

		}

	}
}
