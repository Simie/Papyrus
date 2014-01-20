using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Papyrus.Core;

namespace Papyrus.Tests
{
	[TestClass]
	public class RecordDatabaseTests
	{

		[TestMethod]
		public void TestSimpleSetup()
		{

			var parent = PluginLoader.LoadPluginString(TestPlugins.TestParentPlugin);
			var child = PluginLoader.LoadPluginString(TestPlugins.TestChildPlugin);

			var database = new RecordDatabase(new List<Plugin>() {child, parent});

			// Test record was overriden correctly
			var overriden = database.GetRecord<TestRecord>(new RecordKey(0, "Master"));

			Assert.AreEqual("Overwritten", overriden.TestString);

		}

		[TestMethod]
		public void TestRefGet()
		{

			var parent = PluginLoader.LoadPluginString(TestPlugins.TestParentPlugin);

			var database = new RecordDatabase(new List<Plugin>() { parent });

			var validRecord = new RecordRef<TestRecord>(new RecordKey(0, "Master"));
			var invalidRecord = new RecordRef<TestRecord>(new RecordKey(0, "NonExist"));

			Assert.IsNotNull(database.Get(validRecord));
			Assert.IsNull(database.Get(invalidRecord));

			try {
				database.Get(invalidRecord, true);
				Assert.Fail("Didn't throw exception");
			} catch {}

		}

		[TestMethod]
		public void TestPolymorphicReference()
		{

			var plugin = PluginLoader.LoadPluginString(TestPlugins.TestPolymorphicPlugin);

			var database = new RecordDatabase(new List<Plugin>() {plugin});

			var record1Ref = new RecordRef<TestRecordParent>(new RecordKey(0, "Master"), typeof (TestChild1));
			var record2Ref = new RecordRef<TestRecordParent>(new RecordKey(0, "Master"), typeof (TestChild2));

			var record1 = database.Get(record1Ref);
			var record2 = database.Get(record2Ref);

			Assert.IsNotNull(record1);
			Assert.IsNotNull(record2);

			Assert.IsTrue(record1 is TestChild1);
			Assert.IsTrue(record2 is TestChild2);

		}

		[TestMethod]
		public void TestMissingPlugin()
		{

			var child = PluginLoader.LoadPluginString(TestPlugins.TestChildPlugin);

			try {
				var database = new RecordDatabase(new List<Plugin>() {child});
				Assert.Fail("Didn't throw exception with missing plugin");
			} catch (MissingPluginException e) {
				Assert.IsTrue(e.Plugin == "Master", "Missing plugin name was incorrect");
			}
			

		}

		[TestMethod]
		public void TestEmptyDatabase()
		{

			var database = new RecordDatabase(new List<Plugin>());

		}

	}
}
