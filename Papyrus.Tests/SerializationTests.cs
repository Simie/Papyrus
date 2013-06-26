using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Papyrus.Core;
using Papyrus.Core.Util;

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


			original.AddRecord(new TestRecordOne() { InternalKey = new RecordKey(0, "PluginName")});
			original.AddRecord(new TestRecordOne() { InternalKey = new RecordKey(1, "PluginName")});
			original.AddRecord(new TestRecordTwo() { InternalKey = new RecordKey(0, "PluginName")});
			original.AddRecord(new TestRecordTwo() { InternalKey = new RecordKey(1, "PluginName")});

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

		[TestMethod]
		public void TestRecordRefCollectionSerialization()
		{

			var referenceOne = new RecordRef<TestRecord>(new RecordKey(1337, "TestPlugin"));
			var referenceTwo = new RecordRef<TestRecord>(new RecordKey(1330, "TestPlugin2"));
			var referenceThree = new RecordRef<TestRecord>(new RecordKey(1337, "TestPlugin2"));

			var collection = new RecordRefCollection<TestRecord>(new [] {
				referenceOne, referenceTwo, referenceThree
			});

			var settings = Serialization.GetJsonSettings();

			var json = JsonConvert.SerializeObject(collection, settings);

			Debug.WriteLine(json);

			var loaded = JsonConvert.DeserializeObject<RecordRefCollection<TestRecord>>(json, settings);

			Assert.IsTrue(loaded.Count() == collection.Count());
			Assert.IsTrue(loaded.Contains(referenceOne));
			Assert.IsTrue(loaded.Contains(referenceTwo));
			Assert.IsTrue(loaded.Contains(referenceThree));

		}


	}

}
