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

			var parent = Plugin.FromString(TestPlugins.TestParentPlugin);
			var child = Plugin.FromString(TestPlugins.TestChildPlugin);

			var database = new RecordDatabase(new List<Plugin>() {child, parent});

			// Test record was overriden correctly
			var overriden = database.GetRecord<TestRecord>(new RecordKey(0, "Master"));

			Assert.AreEqual(overriden.TestString, "Overwritten");

		}

		[TestMethod]
		public void TestRefGet()
		{

			var parent = Plugin.FromString(TestPlugins.TestParentPlugin);

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
		public void TestMissingPlugin()
		{

			var child = Plugin.FromString(TestPlugins.TestChildPlugin);

			try {
				var database = new RecordDatabase(new List<Plugin>() {child});
				Assert.Fail("Didn't throw exception with missing plugin");
			} catch (MissingPluginException) {}
			

		}

		[TestMethod]
		public void TestEmptyDatabase()
		{

			var database = new RecordDatabase(new List<Plugin>());

		}

	}
}
