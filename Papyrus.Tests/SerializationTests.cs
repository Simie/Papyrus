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

		[TestMethod]
		public void TestSerialization()
		{

			var recordRef = new RecordRef<TestRecordOne>(new RecordKey(120, "TestPlugin"));

			var settings = Core.Util.Serialization.GetJsonSettings();

			var json = JsonConvert.SerializeObject(recordRef, settings);

			Debug.WriteLine(json);

			var deserRef = JsonConvert.DeserializeObject<RecordRef<TestRecordOne>>(json, settings);

			Assert.AreEqual(recordRef, deserRef);

		}

		[TestMethod]
		public void TestRecordCollectionSerialization()
		{

			var original = new RecordCollection();

			original.AddRecord(new RecordKey(0, "PluginName"), new TestRecordOne());
			original.AddRecord(new RecordKey(1, "PluginName"), new TestRecordOne());
			original.AddRecord(new RecordKey(0, "PluginName"), new TestRecordTwo());
			original.AddRecord(new RecordKey(1, "PluginName"), new TestRecordTwo());

			var settings = Core.Util.Serialization.GetJsonSettings();

			var json = JsonConvert.SerializeObject(original, settings);

			Debug.WriteLine(json);

			var loaded = JsonConvert.DeserializeObject<RecordCollection>(json, settings);

			var originalRecords1 = original.GetRecords<TestRecordOne>();
			var loadedRecords1 = loaded.GetRecords<TestRecordOne>();

			foreach (var rec in originalRecords1) {
				Debug.WriteLine(rec.Key);
			}
			Debug.WriteLine("");
			foreach (var rec in loadedRecords1) {
				Debug.WriteLine(rec.Key);
			}

			Assert.IsTrue(loadedRecords1.IsEquivalentTo(originalRecords1, Record.KeyComparer));

		}


	}

}
