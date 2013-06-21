using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Papyrus.Core;

namespace Papyrus.Tests
{
	[TestClass]
	public class RecordRefTests
	{

		[TestMethod]
		public void TestEquality()
		{

			var ref1 = new RecordRef<TestRecordOne>(new RecordKey(120, "TestPlugin"));
			var ref2 = new RecordRef<TestRecordOne>(new RecordKey(125, "TestPlugin"));
			var ref3 = new RecordRef<TestRecordTwo>(new RecordKey(120, "TestPlugin"));
			var ref4 = new RecordRef<TestRecordTwo>(new RecordKey(120));

			Assert.AreEqual(ref1, ref1);

			Assert.AreNotEqual(ref1, ref2);
			Assert.AreNotEqual(ref1, ref3);
			Assert.AreNotEqual(ref1, ref4);

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

	}
}
