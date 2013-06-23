using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Papyrus.Core;

namespace Papyrus.Tests
{
	[TestClass]
	public class PluginComposerTests
	{

		[TestMethod]
		public void TestBasicAddRetrieve()
		{

			var plugin = PluginComposer.CreateBlank("TestPlugin");

			var testRecord = plugin.CreateRecord<TestRecord>();
			testRecord.SetProperty(() => testRecord.TestBoolean, true);
			testRecord.SetProperty(() => testRecord.TestString, null);

			plugin.SaveRecord(testRecord);

			var fetch = plugin.GetRecord<TestRecord>(testRecord.Key);

			Assert.IsTrue(Record.PropertyComparer.Equals(fetch, testRecord));

		}

	}
}
