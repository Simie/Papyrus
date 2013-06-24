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
	}
}
