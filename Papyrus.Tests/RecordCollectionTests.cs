using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Papyrus.Core;

namespace Papyrus.Tests
{

	[TestClass]
	public class RecordCollectionTests
	{

		[TestMethod]
		public void TestBasicRetrieve()
		{

			var recordCollection = new RecordCollection();

			var testKey1 = new RecordKey(20, "TestPlugin");
			var testRecord1 = new TestRecordOne();

			try {
				recordCollection.GetRecord<TestRecordOne>(testKey1);
				Assert.Fail("Didn't throw exception when record doesn't exist");
			} catch (KeyNotFoundException) { }

			recordCollection.AddRecord(testKey1, testRecord1);

			try {
				Assert.AreSame(recordCollection.GetRecord<TestRecordOne>(testKey1), testRecord1);
			} catch { Assert.Fail("GetRecord threw exception"); }

		}

		[TestMethod]
		public void TestDuplicateKeys()
		{

			var recordCollection = new RecordCollection();

			var testKey1 = new RecordKey(20, "TestPlugin");
			var testRecord1 = new TestRecordOne();
			var testRecord2 = new TestRecordOne();

			recordCollection.AddRecord(testKey1, testRecord1);

			try {
				recordCollection.AddRecord(testKey1, testRecord2);
				Assert.Fail("Adding duplicate key didn't throw exception.");
			} catch(ArgumentException) {}

		}

		[TestMethod]
		public void TestRemoveRecord()
		{

			var recordCollection = new RecordCollection();

			var testKey1 = new RecordKey(20, "TestPlugin");
			var testRecord1 = new TestRecordOne();
			var testRecord2 = new TestRecordTwo();

			recordCollection.AddRecord(testKey1, testRecord1);
			recordCollection.AddRecord(testKey1, testRecord2);

			// Ensure that the record is reported removed
			Assert.IsTrue(recordCollection.RemoveRecord<TestRecordOne>(testKey1));

			Record testFetch;

			// Ensure the record can no longer be fetched
			Assert.IsFalse(recordCollection.TryGetRecord(typeof(TestRecordOne), testKey1, out testFetch));
			Assert.IsNull(testFetch);

			Record testRecord2Ret;

			// Ensure the record with the same key but different type still exists
			Assert.IsTrue(recordCollection.TryGetRecord(typeof (TestRecordTwo), testKey1, out testRecord2Ret));
			Assert.AreSame(testRecord2, testRecord2Ret);

		}

		[TestMethod]
		public void TestSimilarKeys()
		{

			var recordCollection = new RecordCollection();

			var testKey1 = new RecordKey(20, "TestPlugin");
			var testRecord1 = new TestRecordOne();
			var testRecord2 = new TestRecordTwo();

			// Test that the same key retrieves different results when fetching different types
			recordCollection.AddRecord(testKey1, testRecord1);
			recordCollection.AddRecord(testKey1, testRecord2);

			try {
				Assert.AreSame(recordCollection.GetRecord<TestRecordOne>(testKey1), testRecord1);
				Assert.AreSame(recordCollection.GetRecord<TestRecordTwo>(testKey1), testRecord2);
			} catch { Assert.Fail("GetRecord threw exception"); }

		}

		[TestMethod]
		public void TestMergeCollection()
		{

			var collection1 = new RecordCollection();

			var testRecord1 = new TestRecordOne(); var testKey1 = new RecordKey(0);
			var testRecord2 = new TestRecordTwo(); var testKey2 = new RecordKey(1);
			var testRecord3 = new TestRecordOne(); var testKey3 = new RecordKey(2);

			collection1.AddRecord(testKey1, testRecord1);
			collection1.AddRecord(testKey2, testRecord2);
			collection1.AddRecord(testKey3, testRecord3);

			var collection2 = new RecordCollection();

			var testOverrideRecord1 = new TestRecordOne();
			var testOverrideRecord2 = new TestRecordTwo();
			var testNewRecord1 = new TestRecordOne(); var testNewKey1 = new RecordKey(0, "Plugin");
			var testNewRecord2 = new TestRecord(); var testNewKey2 = new RecordKey(0);

			collection2.AddRecord(testKey1, testOverrideRecord1);
			collection2.AddRecord(testKey2, testOverrideRecord2);

			collection2.AddRecord(testNewKey1, testNewRecord1);
			collection2.AddRecord(testNewKey2, testNewRecord2);

			collection1.Merge(collection2);

			// Test that the record was overriden correctly
			Assert.AreSame(collection1.GetRecord<TestRecordOne>(testKey1), testOverrideRecord1);
			Assert.AreSame(collection1.GetRecord<TestRecordTwo>(testKey2), testOverrideRecord2);

			// Test that the non-overriden record is still accessible
			Assert.AreSame(collection1.GetRecord<TestRecordOne>(testKey3), testRecord3);

			// Test that the new (appended) record is accessible
			Assert.AreSame(collection1.GetRecord<TestRecordOne>(testNewKey1), testNewRecord1);

			// Test that the record list that didn't exist in the original collection was correctly copied over
			Assert.AreSame(collection1.GetRecord<TestRecord>(testNewKey2), testNewRecord2);

		}

		[TestMethod]
		public void TestGetRecords()
		{

			var collection = new RecordCollection();

			var record1 = new TestRecordOne();
			var record2 = new TestRecordOne();
			var record3 = new TestRecordTwo();
			var record4 = new TestRecordTwo();

			collection.AddRecord(new RecordKey(0), record1);
			collection.AddRecord(new RecordKey(1), record2);
			collection.AddRecord(new RecordKey(0), record3);
			collection.AddRecord(new RecordKey(1), record4);

			// Check that the generic and non-generic methods work the same (they have slightly different implementation)
			var recordsOneOne = collection.GetRecords<TestRecordOne>().ToList(); // Cast to list for CollectionAssert
			var recordsOneTwo = collection.GetRecords(typeof(TestRecordOne)).ToList();

			CollectionAssert.AreEquivalent(recordsOneOne.ToList(), recordsOneTwo.ToList(), "Record collections differ");

			CollectionAssert.Contains(recordsOneOne, record1);
			CollectionAssert.Contains(recordsOneOne, record2);
			
			CollectionAssert.AllItemsAreInstancesOfType(recordsOneOne, typeof(TestRecordOne));
			Assert.AreEqual(recordsOneOne.Count, 2);

		}

	}
}
