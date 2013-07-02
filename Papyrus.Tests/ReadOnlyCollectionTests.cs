using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Papyrus.Core;

namespace Papyrus.Tests
{
	[TestClass]
	public class ReadOnlyCollectionTests
	{
		[TestMethod]
		public void Test()
		{

			var ourList = new List<int>() {
				1,
				2,
				5,
				6
			};

			var readOnlyList = new ReadOnlyCollection<int>(ourList);

			Assert.IsTrue(readOnlyList.SequenceEqual(ourList));
			Assert.IsTrue(readOnlyList.IsReadOnly);

			try {
				readOnlyList.Add(3);
				Assert.Fail("Didn't throw exception");
			} catch {}
			try {
				readOnlyList.Remove(3);
				Assert.Fail("Didn't throw exception");
			} catch {}
			try {
				readOnlyList.Clear();
				Assert.Fail("Didn't throw exception");
			} catch {}

		}

		[TestMethod]
		public void TestJson()
		{


			var ourList = new List<int>() {
				1,
				2,
				5,
				6
			};

			var list = new ReadOnlyCollection<int>(ourList);

			var settings = Core.Util.Serialization.GetJsonSettings();

			var json = JsonConvert.SerializeObject(list, settings);

			Console.WriteLine(json);

			var restored = JsonConvert.DeserializeObject<ReadOnlyCollection<int>>(json, settings);

			Assert.IsTrue(restored.SequenceEqual(list));

		}

	}
}
