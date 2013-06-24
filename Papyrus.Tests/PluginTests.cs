using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Papyrus.Core;

namespace Papyrus.Tests
{
	[TestClass]
	public class PluginTests
	{

		private const string TestPluginJson = @"{
  ""Name"": ""TestPlugin"",
  ""Records"": {
	""Papyrus.Tests.TestRecord"": [
	  {
		""Key"": ""TestPlugin/000000"",
		""TestBoolean"": true,
		""TestString"": null,
		""TestInteger"": 0,
		""TestReference"": ""000000"",
		""EditorID"": null
	  },
	  {
		""Key"": ""TestPlugin/000001"",
		""TestBoolean"": false,
		""TestString"": ""Test String Value"",
		""TestInteger"": 0,
		""TestReference"": ""000000"",
		""EditorID"": null
	  }
	]
  }
}";

		/// <summary>
		/// Regression test deserialization (format should not change from sample above)
		/// </summary>
		[TestMethod]
		public void TestDeserialization()
		{

			var plugin = Plugin.FromString(TestPluginJson);

			var record1 = plugin.Records.GetRecord<TestRecord>(new RecordKey(0, "TestPlugin"));
			var record2 = plugin.Records.GetRecord<TestRecord>(new RecordKey(1, "TestPlugin"));

			Assert.AreEqual(record1.TestBoolean, true);
			Assert.AreEqual(record1.TestInteger, 0);

			Assert.AreEqual(record2.TestBoolean, false);
			Assert.AreEqual(record2.TestString, "Test String Value");

		}

		/// <summary>
		/// Test record reference parent detection
		/// </summary>
		[TestMethod]
		public void TestParentDetection()
		{

			var plugin = new Plugin("TestPlugin");

			plugin.RefreshParents();

			// Should have no parents with no records present
			Assert.AreEqual(plugin.Parents.Count, 0);

			// Test that adding a record overriden a parent's record is detected correctly
			plugin.Records.AddRecord(new TestRecordOne() { InternalKey = new RecordKey(0, "Parent")});

			plugin.RefreshParents();
			
			Assert.AreEqual(plugin.Parents.Count, 1);
			Assert.IsTrue(plugin.Parents.Contains("Parent"));

			// Test that adding a record with a reference to a plugin is detected correctly
			var testRecord = new TestRecord() {InternalKey = new RecordKey(0, "TestPlugin")};
			testRecord.SetProperty(() => testRecord.TestReference, new RecordRef<TestRecordOne>(new RecordKey(0, "Parent2")));

			plugin.Records.AddRecord(testRecord);

			plugin.RefreshParents();

			Assert.AreEqual(plugin.Parents.Count, 2);
			Assert.IsTrue(plugin.Parents.Contains("Parent2"));

		}

	}
}
