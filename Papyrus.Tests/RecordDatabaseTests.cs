using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Papyrus.Core;

namespace Papyrus.Tests
{
	[TestClass]
	public class RecordDatabaseTests
	{

		private const string TestParentPlugin = 
@"{
	""Name"": ""Master"",
	""Records"": {
	""Papyrus.Tests.TestRecord"": [
		{
		""Key"": ""Master/000000"",
		""TestBoolean"": true,
		""TestString"": ""Original Value"",
		""TestInteger"": 0,
		""TestReference"": ""000000"",
		""EditorID"": null
		},
		{
		""Key"": ""Master/000001"",
		""TestBoolean"": false,
		""TestString"": ""Test String Value"",
		""TestInteger"": 0,
		""TestReference"": ""000000"",
		""EditorID"": null
		}
	]
	}
}";
		private const string TestChildPlugin = 
@"{
	""Name"": ""Child"",
	""Records"": {
	""Papyrus.Tests.TestRecord"": [
		{
		""Key"": ""Master/000000"",
		""TestBoolean"": true,
		""TestString"": ""Overwritten"",
		""TestInteger"": 0,
		""TestReference"": ""000000"",
		""EditorID"": null
		},
		{
		""Key"": ""Child/000000"",
		""TestBoolean"": false,
		""TestString"": ""New Record"",
		""TestInteger"": 0,
		""TestReference"": ""000000"",
		""EditorID"": null
		}
	]
	}
}";

		[TestMethod]
		public void TestMethod1()
		{

			var parent = Plugin.FromString(TestParentPlugin);
			var child = Plugin.FromString(TestChildPlugin);

			var database = new RecordDatabase(new List<Plugin>() {parent, child});

		}
	}
}
