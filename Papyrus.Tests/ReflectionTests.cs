using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Papyrus.Core;
using Papyrus.Core.Util;

namespace Papyrus.Tests
{
	[TestClass]
	public class ReflectionTests
	{

		[TestMethod]
		public void TestGetPropertyInfo()
		{
			
			Assert.IsTrue(ReflectionUtil.GetWritablePropertyInfo(typeof(TestRecord), "EditorID").CanWrite);

		}

		[TestMethod]
		public void TestPropertyDetection()
		{

			var properties = Core.Util.RecordReflectionUtil.GetProperties<TestRecord>();

			// Test inherited property
			Assert.IsTrue(properties.Any(p => p.Name == "EditorID"));

			Assert.IsTrue(properties.Any(p => p.Name == "TestBoolean"));
			Assert.IsTrue(properties.Any(p => p.Name == "TestString"));
			Assert.IsTrue(properties.Any(p => p.Name == "TestInteger"));
			Assert.IsTrue(properties.Any(p => p.Name == "TestReference"));

			Assert.IsFalse(properties.Any(p => p.Name == "ShouldIgnore"));

			Assert.AreEqual(properties.Count, 5);

		}

		[TestMethod]
		public void TestPropertyDiff()
		{

			// Test inherited property
			TestPropertyDiff("EditorID", "Value1", "Value2");

			TestPropertyDiff("TestBoolean", false, true);
			TestPropertyDiff("TestString", "Old", "Changed");
			TestPropertyDiff("TestInteger", 1337, 20000);

			var ref1 = new RecordRef<TestRecordOne>(new RecordKey(10, "Plugin"));
			var ref2 = new RecordRef<TestRecordOne>(new RecordKey(11, "Plugin"));


			TestPropertyDiff("TestReference", ref1, ref2);

		}

		private void TestPropertyDiff(string prop, object oldValue, object newValue)
		{

			Debug.WriteLine("Testing {0}", (object)prop);

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

		[TestMethod]
		public void TestTypeNameResolve()
		{

			TestType(typeof(TestRecordOne));
			TestType(typeof(TestRecordTwo));


			TestType(typeof(object), false);

		}

		void TestType(Type type, bool expectTrue = true)
		{

			var name = type.FullName;

			var resolvedType = Core.Util.ReflectionUtil.ResolveRecordType(name);

			if (expectTrue) {

				Assert.IsNotNull(resolvedType);
				Assert.AreEqual(resolvedType, type);

			} else {

				Assert.IsNull(resolvedType);

			}

		}

		[TestMethod]
		public void TestRecordClone()
		{

			var record = new TestRecord();
			record.SetProperty(() => record.TestBoolean, true);
			record.SetProperty(() => record.TestInteger, 10);
			record.SetProperty(() => record.TestString, "Test String");
			record.SetProperty(() => record.TestReference, new RecordRef<TestRecordOne>(new RecordKey(1337, "TestPlugin")));
			record.ShouldIgnore = 122;

			var clone = (TestRecord)record.Clone();

			Assert.AreEqual(record.TestBoolean, clone.TestBoolean);
			Assert.AreEqual(record.TestInteger, clone.TestInteger);
			Assert.AreEqual(record.TestString, clone.TestString);
			Assert.AreEqual(record.TestReference, clone.TestReference);

			Assert.AreEqual(clone.ShouldIgnore, default(int));

		}

	}

}
