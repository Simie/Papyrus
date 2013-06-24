using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papyrus.Tests
{
	public static class TestPlugins
	{

		public const string TestParentPlugin =
			@"{
	""Name"": ""Master"",
	""Parents"": [],
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

		public const string TestChildPlugin =
			@"{
	""Name"": ""Child"",
	""Parents"": [""Master""],
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

	}
}
