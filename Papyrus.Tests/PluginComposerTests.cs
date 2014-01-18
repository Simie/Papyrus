using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Papyrus.Core;
using Papyrus.Core.Util;

namespace Papyrus.Tests
{
	[TestClass]
	public class PluginComposerTests
	{

		[TestMethod]
		public void TestMissingParent()
		{

			var testChild = PluginLoader.LoadPluginString(TestPlugins.TestChildPlugin);

			try {
				var composer = PluginComposer.EditPlugin(testChild, new List<Plugin>());
				Assert.Fail("Exception wasn't thrown with missing parent");
			} catch (MissingPluginException) { }

		}

		[TestMethod]
		public void TestBasicAddRetrieve()
		{

			var plugin = PluginComposer.CreateBlank("TestPlugin");

			Assert.IsFalse(plugin.NeedSaving);

			var testRecord = plugin.CreateRecord<TestRecord>();
			testRecord.SetProperty(() => testRecord.TestBoolean, true);
			testRecord.SetProperty(() => testRecord.TestString, null);

			plugin.SaveRecord(testRecord);

			var fetch = plugin.GetRecord<TestRecord>(testRecord.Key);

			// Check that properties in saved record match the record we created
			Assert.IsTrue(Record.PropertyComparer.Equals(fetch, testRecord));

			var pluginJson = PluginSerializer.ToJson(plugin.Plugin);

			Debug.WriteLine(pluginJson);

			Assert.IsTrue(plugin.NeedSaving);

		}

		[TestMethod]
		public void TestEvent()
		{
			
			// Test RecordListChanged event

			var composer = PluginComposer.CreateBlank("Test");

			bool called = false;

			composer.RecordListChanged += (sender, args) => called = true;

			var record = composer.CreateRecord<TestRecord>();

			Assert.IsTrue(called);

			//composer.DeleteRecord(record.GetType(), record.Key);

		}

	}
}
