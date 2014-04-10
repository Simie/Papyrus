using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Papyrus.Core;
using Papyrus.Core.Util;

namespace Papyrus.Tests.Regressions
{

	[TestClass]
	public class NestedRecordCollectionTest
	{

		class TestRecord : Record
		{

			public ReadOnlyCollection<TestCollectionEntry> TestCollection { get; set; } 

		}

		class TestNestedRecord : Record
		{

			

		}

		class TestCollectionEntry
		{

			public RecordRefCollection<TestNestedRecord> NestedCollection { get; set; } 

		}

		[TestMethod]
		public void Test()
		{

			var r = new TestRecord();

			r.SetProperty(() => r.TestCollection, new ReadOnlyCollection<TestCollectionEntry>(new TestCollectionEntry[] {
				new TestCollectionEntry() {
					NestedCollection =
						new RecordRefCollection<TestNestedRecord>(new[]
						{new RecordRef<TestNestedRecord>(), new RecordRef<TestNestedRecord>(),})
				},
				new TestCollectionEntry() {
					NestedCollection =
						new RecordRefCollection<TestNestedRecord>(new[]
						{new RecordRef<TestNestedRecord>(), new RecordRef<TestNestedRecord>(),})
				}
			}));

			var json = RecordSerializer.ToJson(r);
			Debug.WriteLine(json);

		}

	}

}
