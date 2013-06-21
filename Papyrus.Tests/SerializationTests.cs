using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Papyrus.Core;

namespace Papyrus.Tests
{

	[TestClass]
	public class SerializationTests
	{

		[TestMethod]
		public void TestRecordKeySerialization()
		{

			var key = new RecordKey(250, "TestPlugin");

			var settings = Core.Util.Serialization.GetJsonSettings();

			var json = JsonConvert.SerializeObject(key, settings);

			Debug.WriteLine(json);

			var deKey = JsonConvert.DeserializeObject<RecordKey>(json, settings);

			Assert.AreEqual(key, deKey);

		}

		[TestMethod]
		public void TestNullPluginRecordKeySerialization()
		{

			var key = new RecordKey(100);

			var settings = Core.Util.Serialization.GetJsonSettings();

			var json = JsonConvert.SerializeObject(key, settings);

			Debug.WriteLine(json);

			var deKey = JsonConvert.DeserializeObject<RecordKey>(json, settings);

			Assert.AreEqual(key, deKey);

		}

	}

}
