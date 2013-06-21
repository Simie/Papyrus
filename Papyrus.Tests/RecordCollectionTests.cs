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
			var testRecord1 = new TestRecord1();

			try {
				recordCollection.GetRecord<TestRecord1>(testKey1);
				Assert.Fail("Didn't throw exception when record doesn't exist");
			} catch (KeyNotFoundException) { }

			recordCollection.AddRecord(testKey1, testRecord1);

			try {
				Assert.AreSame(recordCollection.GetRecord<TestRecord1>(testKey1), testRecord1);
			} catch { Assert.Fail("GetRecord threw exception"); }

		}

		[TestMethod]
		public void TestDuplicateKeys()
		{

			var recordCollection = new RecordCollection();

			var testKey1 = new RecordKey(20, "TestPlugin");
			var testRecord1 = new TestRecord1();
			var testRecord2 = new TestRecord1();

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
			var testRecord1 = new TestRecord1();
			var testRecord2 = new TestRecord2();

			recordCollection.AddRecord(testKey1, testRecord1);
			recordCollection.AddRecord(testKey1, testRecord2);

			// Ensure that the record is reported removed
			Assert.IsTrue(recordCollection.RemoveRecord<TestRecord1>(testKey1));

			Record testRecord2Ret;

			// Ensure the record with the same key but different type still exists
			Assert.IsTrue(recordCollection.TryGetRecord(typeof (TestRecord2), testKey1, out testRecord2Ret));
			Assert.AreSame(testRecord2, testRecord2Ret);

		}

		[TestMethod]
		public void TestSimilarKeys()
		{

			var recordCollection = new RecordCollection();

			var testKey1 = new RecordKey(20, "TestPlugin");
			var testRecord1 = new TestRecord1();
			var testRecord2 = new TestRecord2();

			// Test that the same key retrieves different results when fetching different types
			recordCollection.AddRecord(testKey1, testRecord1);
			recordCollection.AddRecord(testKey1, testRecord2);

			try {
				Assert.AreSame(recordCollection.GetRecord<TestRecord1>(testKey1), testRecord1);
				Assert.AreSame(recordCollection.GetRecord<TestRecord2>(testKey1), testRecord2);
			} catch { Assert.Fail("GetRecord threw exception"); }

		}

	}
}
