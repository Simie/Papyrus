using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Papyrus.Core;

namespace Papyrus.Tests
{
	[TestClass]
	public class ReflectionTests
	{

		class TestRecord : Record
		{

			public bool TestBoolean { get; private set; }

			public string TestString { get; private set; }

			public int TestInteger { get; private set; }

			[Newtonsoft.Json.JsonIgnore]
			public int ShouldIgnore { get; private set; }

		}

		[TestMethod]
		public void TestPropertyDetection()
		{

			var properties = Core.Util.RecordReflectionUtil.GetProperties<TestRecord>();

			Assert.IsTrue(properties.Any(p => p.Name == "TestBoolean"));
			Assert.IsTrue(properties.Any(p => p.Name == "TestString"));
			Assert.IsTrue(properties.Any(p => p.Name == "TestInteger"));

			Assert.IsFalse(properties.Any(p => p.Name == "ShouldIgnore"));

			Assert.AreEqual(properties.Count, 3);

		}

		[TestMethod]
		public void TestPropertyDiff()
		{

			TestPropertyDiff("TestBoolean", false, true);
			TestPropertyDiff("TestString", "Old", "Changed");
			TestPropertyDiff("TestInteger", 1337, 20000);

		}

		private void TestPropertyDiff(string prop, object oldValue, object newValue)
		{

			var record1 = new TestRecord();
			var record2 = new TestRecord();
		
			record1.SetProperty(prop, oldValue);
			record2.SetProperty(prop, newValue);

			var differences = Core.Util.RecordDiffUtil.Diff(record1, record2);

			// Check that the difference is correctly detected
			Assert.IsTrue(
				differences.Any(p => p.Property.Name == prop && p.OldValue.Equals(oldValue) && p.NewValue.Equals(newValue)),
				"False-negative for property {0} values [{1}], [{2}]", prop, oldValue, newValue);

			record2.SetProperty(prop, oldValue);

			differences = Core.Util.RecordDiffUtil.Diff(record1, record2);

			// Check that there is no false positive
			Assert.IsFalse(
				differences.Any(p => p.Property.Name == prop),
				"False-positive for property {0} with value [{1}]", prop, oldValue);

		}

	}
}
