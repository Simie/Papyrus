using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
			testRecord.TestBoolean = true;
			testRecord.TestString = null;

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

		/// <summary>
		/// Test creating records, then cloning a few for editing, then saving.
		/// </summary>
		[TestMethod]
		public void TestChildCreateCloneSave()
		{

			Plugin parentPlugin;

			{
				// Create a parent plugin
				var composer = PluginComposer.CreateBlank("TestParent");

				// Create a few records
				composer.CreateRecord<TestRecord>();
				composer.CreateRecord<TestRecord>();
				composer.CreateRecord<TestRecord>();

				StringBuilder json = new StringBuilder();
				using (var s = new StringWriter(json))
					composer.SavePlugin(s);

				parentPlugin = PluginLoader.LoadPluginString(json.ToString());

			}

			var child = PluginComposer.CreateChild("TestChild", new List<Plugin> {parentPlugin});

			var rec = child.GetEditableRecord<TestRecord>(new RecordKey(0, "TestParent"));

			rec.TestString = "TestValue";

			child.SaveRecord(rec);

			// Force GetMergedCollection() to be called
			child.GetRecords<TestRecord>();

			var cJson = new StringBuilder();
			using (var s = new StringWriter(cJson))
				child.SavePlugin(s);

			Console.WriteLine(cJson);

		}

		/// <summary>
		/// Test that changes to a record object are applied to any publicly retreived record objects
		/// </summary>
		[TestMethod]
		public void TestRecordSavePropagation()
		{

			var parentPlugin = PluginLoader.LoadPluginString(TestPlugins.TestParentPlugin);
			var childPlugin = PluginLoader.LoadPluginString(TestPlugins.TestChildPlugin);

			var composer = PluginComposer.EditPlugin(childPlugin, new[] {parentPlugin});
			var key = new RecordKey("Master/000001");

			// Retrieve record from composer. This should be fetching the record from the Master plugin
			var masterRecord = composer.GetRecord<TestRecord>(key);

			Assert.AreEqual("Test String Value", masterRecord.TestString);

			// Now make a change to the record in the active plugin
			{
				var childRecord = composer.GetEditableRecord<TestRecord>(key);
				childRecord.TestString = "Modified String Value";
				composer.SaveRecord(childRecord);
			}

			// Check that the modified value is correctly applied to the existing object
			Assert.AreEqual("Modified String Value", masterRecord.TestString);


		}

		/// <summary>
		/// Test that a Record saved with no changes from parent will not be added to the active plugin (and will be removed)
		/// </summary>
		[TestMethod]
		public void TestNoDiffRecordSave()
		{

			var parentPlugin = PluginLoader.LoadPluginString(TestPlugins.TestParentPlugin);
			var childPlugin = PluginLoader.LoadPluginString(TestPlugins.TestChildPlugin);

			var composer = PluginComposer.EditPlugin(childPlugin, new[] { parentPlugin });
			var key = new RecordKey("Master/000001");

			// Internal functions ahead

			Record rec;

			// Test that plugin does not contain the parent record
			Assert.IsFalse(composer.Plugin.Records.TryGetRecord(typeof(TestRecord), key, out rec));

			// Save unchanged record
			{
				var edit = composer.GetEditableRecord<TestRecord>(key);
				composer.SaveRecord(edit);
			}

			// Test that plugin does not contain the parent record
			Assert.IsFalse(composer.Plugin.Records.TryGetRecord(typeof(TestRecord), key, out rec));

			// Make a change which should cause the record to be added to the active plugin
			{
				var edit = composer.GetEditableRecord<TestRecord>(key);
				edit.TestString = "Modified String Value";
				composer.SaveRecord(edit);
			}

			// Test that plugin does contain the parent record
			Assert.IsTrue(composer.Plugin.Records.TryGetRecord(typeof(TestRecord), key, out rec));
			Assert.AreEqual("Modified String Value", composer.GetRecord<TestRecord>(key).TestString);

			// Revert the change, which should cause the record to be removed from the active plugin
			{
				var edit = composer.GetEditableRecord<TestRecord>(key);
				edit.TestString = "Test String Value";
				composer.SaveRecord(edit);
			}

			// Test that plugin does not contain the parent record
			Assert.IsFalse(composer.Plugin.Records.TryGetRecord(typeof(TestRecord), key, out rec));
			Assert.AreEqual("Test String Value", composer.GetRecord<TestRecord>(key).TestString);

			// Make a change which should cause the record to be added to the active plugin
			{
				var edit = composer.GetEditableRecord<TestRecord>(key);
				edit.TestString = "Modified String Value";
				composer.SaveRecord(edit);
			}

			// Test that plugin does contain the parent record
			Assert.IsTrue(composer.Plugin.Records.TryGetRecord(typeof(TestRecord), key, out rec));
			Assert.AreEqual("Modified String Value", composer.GetRecord<TestRecord>(key).TestString);

		}

		/// <summary>
		/// Test that an unchanged record from a parent plugin being saved does not get cause it to be added to the active plugin
		/// </summary>
		[TestMethod]
		public void TestUnchangedRecordSave()
		{

			var parentPlugin = PluginLoader.LoadPluginString(TestPlugins.TestParentPlugin);

			var composer = PluginComposer.CreateChild("Child", new[] { parentPlugin });
			var key = new RecordKey("Master/000001");

			var orig = composer.GetRecord<TestRecord>(key);

			composer.SaveRecord(composer.GetEditableRecord<TestRecord>(key));

			Record rec;
			Assert.IsFalse(composer.Plugin.Records.TryGetRecord(typeof(TestRecord), key, out rec));

		}

		[TestMethod]
		public void TestPolymorphicReference()
		{

			var plugin = PluginLoader.LoadPluginString(TestPlugins.TestPolymorphicPlugin);

			var database = PluginComposer.CreateChild("Test", new [] {plugin});

			var record1Ref = new RecordRef<TestRecordParent>(new RecordKey(0, "Master"), typeof(TestChild1));
			var record2Ref = new RecordRef<TestRecordParent>(new RecordKey(0, "Master"), typeof(TestChild2));

			var record1 = database.Get(record1Ref);
			var record2 = database.Get(record2Ref);

			Assert.IsNotNull(record1);
			Assert.IsNotNull(record2);

			Assert.IsTrue(record1 is TestChild1);
			Assert.IsTrue(record2 is TestChild2);

		}

		[TestMethod]
		public void TestRemoveRecord()
		{

			var plugin = PluginLoader.LoadPluginString(TestPlugins.TestParentPlugin);

			bool eventFired = false;

			var composer = PluginComposer.EditPlugin(plugin);
			composer.RecordListChanged += (sender, args) => {
				eventFired = true;
			};

			var records = composer.GetRecords<TestRecord>().ToList();

			composer.DeleteRecord(records[0]);

			Assert.IsFalse(composer.GetRecords<TestRecord>().Contains(records[0]),
				"Deleted Record should not be returned by GetRecords");	
			
			Assert.IsTrue(composer.GetRecords<TestRecord>().Contains(records[1]),
				"Non-Deleted Record should be returned by GetRecords");

			Assert.IsTrue(eventFired, "RecordListchanged event should fire");

		}


		[TestMethod]
		public void TestRemoveRecordFromParent()
		{

			var parentPlugin = PluginLoader.LoadPluginString(TestPlugins.TestParentPlugin);
			var childPlugin = PluginLoader.LoadPluginString(TestPlugins.TestChildPlugin);

			var composer = PluginComposer.EditPlugin(childPlugin, new[] {parentPlugin});

			var records = composer.GetRecords<TestRecord>().ToList();

			try {

				composer.DeleteRecord(records[0]);
				Assert.Fail("DeleteRecord should have thrown an exception");

			} catch (InvalidOperationException e) {
				// OK
			} catch {
				Assert.Fail("DeleteRecord should throw InvalidOperationException");
			}


			Assert.IsTrue(composer.GetRecords<TestRecord>().Contains(records[0]),
				"Non-Deleted Record should be returned by GetRecords");			
			
			Assert.IsTrue(composer.GetRecords<TestRecord>().Contains(records[1]),
				"Non-Deleted Record should be returned by GetRecords");

		}

	}
}
