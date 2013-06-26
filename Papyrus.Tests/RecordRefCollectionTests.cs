using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Papyrus.Core;

namespace Papyrus.Tests
{
	[TestClass]
	public class RecordRefCollectionTests
	{

		[TestMethod]
		public void TestBasicCreation()
		{

			var collection = new RecordRefCollection<TestRecord>(new RecordRef<TestRecord>[] {
				new RecordRef<TestRecord>(new RecordKey(0, "TestPlugin")),
				new RecordRef<TestRecord>(new RecordKey(1, "TestPlugin"))
			});

			Assert.IsTrue(collection.Contains(new RecordRef<TestRecord>(new RecordKey(0, "TestPlugin"))));
			Assert.IsTrue(collection.Contains(new RecordRef<TestRecord>(new RecordKey(1, "TestPlugin"))));

		}

		[TestMethod]
		public void TestEquality()
		{

			var collection = new RecordRefCollection<TestRecord>(new [] {
				new RecordRef<TestRecord>(new RecordKey(0, "TestPlugin")),
				new RecordRef<TestRecord>(new RecordKey(1, "TestPlugin"))
			});

			var collection2 = new RecordRefCollection<TestRecordOne>(new [] {
				new RecordRef<TestRecordOne>(new RecordKey(0, "TestPlugin")),
				new RecordRef<TestRecordOne>(new RecordKey(1, "TestPlugin"))
			});		
			
			var collection3 = new RecordRefCollection<TestRecordOne>(new [] {
				new RecordRef<TestRecordOne>(new RecordKey(0, "TestPlugin")),
				new RecordRef<TestRecordOne>(new RecordKey(1, "TestPlugin"))
			});

			Assert.AreNotEqual(collection, collection2);
			Assert.AreEqual(collection2, collection3);

		}

	}
}
