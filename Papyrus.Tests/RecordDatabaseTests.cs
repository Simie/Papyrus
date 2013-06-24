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
		public void TestMissingPlugin()
		{

			var child = Plugin.FromString(TestPlugins.TestChildPlugin);

			try {
				var database = new RecordDatabase(new List<Plugin>() {child});
				Assert.Fail("Didn't throw exception with missing plugin");
			} catch (MissingPluginException) {}
			

		}

	}
}
