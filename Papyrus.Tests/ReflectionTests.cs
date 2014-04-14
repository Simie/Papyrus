using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
			Assert.IsTrue(properties.Count(p => p.Name == "EditorID") == 1);

			Assert.IsTrue(properties.Count(p => p.Name == "TestBoolean") == 1);
			Assert.IsTrue(properties.Count(p => p.Name == "TestString") == 1);
			Assert.IsTrue(properties.Count(p => p.Name == "TestInteger") == 1);
			Assert.IsTrue(properties.Count(p => p.Name == "TestReference") == 1);

			Assert.IsTrue(properties.Count(p => p.Name == "ShouldIgnoreReadOnlyString") == 0);

			Assert.IsTrue(properties.Count(p => p.Name == "ShouldIgnore") == 0);

			Assert.AreEqual(properties.Count, 6);

		}


		[TestMethod]
		public void TestPropertyDetectionUnique()
		{

			var properties = Core.Util.RecordReflectionUtil.GetProperties<TestChild1>();

			// Test inherited property
			Assert.IsTrue(properties.Count(p => p.Name == "EditorID") == 1);
			Assert.IsTrue(properties.Count(p => p.Name == "TestFloat") == 1);
			Assert.IsTrue(properties.Count(p => p.Name == "TestChildProperty1") == 1);

			Assert.AreEqual(properties.Count, 3);

		}	
		
		[TestMethod]
		public void TestPropertyDetectionReadOnly()
		{

			var properties = Core.Util.RecordReflectionUtil.GetProperties<TestRecord>(true);

			Debug.WriteLine(string.Join(", ", properties.Select(p => p.Name)));

			// Test inherited property
			Assert.IsTrue(properties.Count(p => p.Name == "EditorID") == 1);

			Assert.IsTrue(properties.Count(p => p.Name == "TestBoolean") == 1);
			Assert.IsTrue(properties.Count(p => p.Name == "TestString") == 1);
			Assert.IsTrue(properties.Count(p => p.Name == "TestInteger") == 1);
			Assert.IsTrue(properties.Count(p => p.Name == "TestReference") == 1);
			Assert.IsTrue(properties.Count(p => p.Name == "EnumFlagsTest") == 1);

			Assert.IsTrue(properties.Count(p => p.Name == "ShouldIgnoreReadOnlyString") == 1);

			Assert.IsTrue(properties.Count(p => p.Name == "ShouldIgnore") == 0);

			Assert.AreEqual(7, properties.Count);

		}	
		
		[TestMethod]
		public void TestGenericPropertyDetection()
		{

			var p1 = Core.Util.RecordReflectionUtil.GetProperties<TestRecord>();

			var p2 = Core.Util.RecordReflectionUtil.GetProperties(typeof(TestRecord));

			CollectionAssert.AreEquivalent(p1, p2);

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

		/// <summary>
		/// Test that null doesn't provide a false positive
		/// </summary>
		[TestMethod]
		public void TestNullComparisonDiff()
		{

			TestPropertyDiff("TestString", null, "TestString");

		}


		/// <summary>
		/// Test that null doesn't provide a false positive
		/// </summary>
		[TestMethod]
		public void TestCollectionDiff()
		{

			// Original
			var c1 = new Core.ReadOnlyCollection<TestCollectionEntry>(new[] {
				new TestCollectionEntry() { Add = 12, IsVisible = false },
				new TestCollectionEntry() { Add = 5, IsVisible = true}
			});

			// New entry
			var c2 = new Core.ReadOnlyCollection<TestCollectionEntry>(new[] {
				new TestCollectionEntry() { Add = 12, IsVisible = false },
				new TestCollectionEntry() { Add = 5, IsVisible = true},
				new TestCollectionEntry() { Add = 8, IsVisible = true}
			});

			// Modified entry
			var c3 = new Core.ReadOnlyCollection<TestCollectionEntry>(new[] {
				new TestCollectionEntry() { Add = 10, IsVisible = false },
				new TestCollectionEntry() { Add = 5, IsVisible = true},
			});


			var r1 = new TestCollectionRecord();
			var r2 = new TestCollectionRecord();

			Assert.AreEqual(0, RecordDiffUtil.Diff(r1, r2).Count, "Should be no differences in freshly created records");

			r1.Attributes = c1;
			r2.Attributes = c1;

			Assert.AreEqual(0, RecordDiffUtil.Diff(r1, r2).Count, "Should be no differences");

			{

				r2.Attributes = c2;

				var diffs = RecordDiffUtil.Diff(r1, r2);

				Assert.AreEqual(1, diffs.Count, "Should be one difference");
				Assert.AreEqual("Attributes", diffs[0].Property.Name);

			}

			{

				r2.Attributes = c3;

				var diffs = RecordDiffUtil.Diff(r1, r2);

				Assert.AreEqual(1, diffs.Count, "Should be one difference");
				Assert.AreEqual("Attributes", diffs[0].Property.Name);

			}

		}

		/// <summary>
		/// Test that null doesn't provide a false positive
		/// </summary>
		[TestMethod]
		public void TestRecordRefCollectionDiff()
		{

			// Original
			var c1 = new RecordRefCollection<TestRecordOne>(new[] {
				new RecordRef<TestRecordOne>(new RecordKey(0, "TestParent")),
				new RecordRef<TestRecordOne>(new RecordKey(1, "TestParent"))
			});

			// Added entry
			var c2 = new RecordRefCollection<TestRecordOne>(new[] {
				new RecordRef<TestRecordOne>(new RecordKey(0, "TestParent")),
				new RecordRef<TestRecordOne>(new RecordKey(1, "TestParent")),
				new RecordRef<TestRecordOne>(new RecordKey(2, "TestParent"))
			});

			// Modified entry
			var c3 = new RecordRefCollection<TestRecordOne>(new[] {
				new RecordRef<TestRecordOne>(new RecordKey(0, "TestParent")),
				new RecordRef<TestRecordOne>(new RecordKey(2, "TestParent")),
			});

			var r1 = new TestRecordCollectionRecord();
			var r2 = new TestRecordCollectionRecord();

			Assert.AreEqual(0, RecordDiffUtil.Diff(r1, r2).Count, "Should be no differences in freshly created records");

			r1.TestRecords = c1;
			r2.TestRecords = c1;

			Assert.AreEqual(0, RecordDiffUtil.Diff(r1, r2).Count, "Should be no differences");

			{
				r2.TestRecords = c2;

				var diffs = RecordDiffUtil.Diff(r1, r2);

				Assert.AreEqual(1, diffs.Count, "Should be one difference");
				Assert.AreEqual("TestRecords", diffs[0].Property.Name);
			}
			{
				r2.TestRecords = c3;

				var diffs = RecordDiffUtil.Diff(r1, r2);

				Assert.AreEqual(1, diffs.Count, "Should be one difference");
				Assert.AreEqual("TestRecords", diffs[0].Property.Name);
			}

		}

		private void TestPropertyDiff<T>(string prop, T oldValue, T newValue)
		{

			Debug.WriteLine("Testing {0}", (object)prop);

			var record1 = new TestRecord();
			var record2 = new TestRecord();
		
			record1.SetProperty(prop, oldValue);
			record2.SetProperty(prop, newValue);

			var differences = Core.Util.RecordDiffUtil.Diff(record1, record2);

			var comparer = EqualityComparer<T>.Default;

			// Check that the difference is correctly detected
			Assert.IsTrue(
				differences.Any(p => p.Property.Name == prop && comparer.Equals(oldValue, (T)p.OldValue) && comparer.Equals((T)p.NewValue, newValue)),
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
			record.TestBoolean = true;
			record.TestInteger = 10;
			record.TestString = "Test String";
			record.TestReference = new RecordRef<TestRecordOne>(new RecordKey(1337, "TestPlugin"));
			record.ShouldIgnore = 122;

			var clone = (TestRecord)record.Clone();

			Assert.AreEqual(record.TestBoolean, clone.TestBoolean);
			Assert.AreEqual(record.TestInteger, clone.TestInteger);
			Assert.AreEqual(record.TestString, clone.TestString);
			Assert.AreEqual(record.TestReference, clone.TestReference);

			Assert.AreEqual(clone.ShouldIgnore, default(int));

		}

		[TestMethod]
		public void TestReferencePropertyDetection()
		{

			var properties = RecordReflectionUtil.GetReferenceProperties(typeof (TestRecord));

			Assert.AreEqual(properties.Count, 1);
			Assert.AreEqual(properties.Single().Name, "TestReference");

		}

		[TestMethod]
		public void TestReferenceCollectionPropertyDetection()
		{

			var properties = RecordReflectionUtil.GetReferenceCollectionProperties(typeof (TestRecordCollectionRecord));

			Assert.AreEqual(properties.Count, 1);
			Assert.AreEqual(properties.Single().Name, "TestRecords");

		}

		[TestMethod]
		public void TestReferencePropertyRetrieval()
		{

			var testRecord = new TestRecord();

			var reference = new RecordRef<TestRecordOne>(new RecordKey(1337, "TestPlugin"));
			testRecord.TestReference = reference;

			var references = RecordUtils.GetReferences(testRecord);

			Assert.AreEqual(references.Count, 1);
			Assert.AreEqual(references.Single(), reference);

		}
	
		[TestMethod]
		public void TestReferenceCollectionPropertyRetrieval()
		{

			var testRecord = new TestRecordCollectionRecord();

			var refOne = new RecordRef<TestRecordOne>(new RecordKey(1337, "TestPlugin"));
			var refTwo = new RecordRef<TestRecordOne>(new RecordKey(1338, "TestPlugin"));
			testRecord.TestRecords = new RecordRefCollection<TestRecordOne>(new [] {refOne, refTwo});

			var references = RecordUtils.GetReferences(testRecord);

			Assert.AreEqual(2, references.Count);
			Assert.IsTrue(references.Contains(refOne));
			Assert.IsTrue(references.Contains(refTwo));

		}

		/// <summary>
		/// Test scanning AppDomain for record types
		/// </summary>
		[TestMethod]
		public void TestRecordTypeDetection()
		{

			var types = RecordReflectionUtil.GetRecordTypes();

			CollectionAssert.Contains(types, typeof(TestRecordOne));
			CollectionAssert.Contains(types, typeof(TestRecordTwo));
			CollectionAssert.Contains(types, typeof(TestRecord));

		}

	}

}
