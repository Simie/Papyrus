using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Papyrus.Core;

namespace Papyrus.Tests
{
	[TestClass]
	public class RecordRefCollectionTests
	{

		[TestMethod]
		public void TestBasicCreation()
		{

			var collection = new RecordRefCollection<TestRecord>(new RecordRef<TestRecord>[] {
				new RecordRef<TestRecord>(new RecordKey(0, "TestPlugin")),
				new RecordRef<TestRecord>(new RecordKey(1, "TestPlugin"))
			});

			Assert.IsTrue(collection.Contains(new RecordRef<TestRecord>(new RecordKey(0, "TestPlugin"))));
			Assert.IsTrue(collection.Contains(new RecordRef<TestRecord>(new RecordKey(1, "TestPlugin"))));

		}

	}
}
