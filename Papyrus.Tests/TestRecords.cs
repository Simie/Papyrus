using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Papyrus.Core;

namespace Papyrus.Tests
{

	class TestRecord : Record
	{


		public bool TestBoolean { get; private set; }

		public string TestString { get; private set; }

		public int TestInteger { get; private set; }

		public RecordRef<TestRecordOne> TestReference { get; private set; }
			
		[Newtonsoft.Json.JsonIgnore]
		public int ShouldIgnore { get; set; }

		public string ShouldIgnoreReadOnlyString { get { return "Test"; } }


	}

	class TestRecordOne : Record
	{



	}

	class TestRecordTwo : Record
	{



	}

	class TestRecordCollectionRecord : Record
	{

		public RecordRefCollection<TestRecordOne> TestRecords { get; private set; }

	}

	public abstract class TestRecordParent : Record
	{

		public float TestFloat { get; private set; }

	}

	public class TestChild1 : TestRecordParent
	{

		public int TestChildProperty1 { get; private set; }

	}

	public class TestChild2 : TestRecordParent
	{

		public string TestChildProperty2 { get; private set; }
		
	}

	public class TestPolymorphicRecord : Record
	{

		public RecordRef<TestRecordParent> TestRef1 { get; private set; } 
		public RecordRef<TestRecordParent> TestRef2 { get; private set; }

		public RecordRefCollection<TestRecordParent> TestReferenceList { get; private set; } 

	}

	public class TestCollectionEntry
	{

		//private float _multiply = 1;
		private bool _isVisible = true;

		public TestCollectionEntry()
		{
			Add = 0;
		}


		public float Add { get; set; }

		public bool IsVisible
		{
			get { return _isVisible; }
			set { _isVisible = value; }
		}

		public override string ToString()
		{
			return String.Format("{0}", Add);
		}

	}

	public class TestCollectionRecord : Record
	{

		public ReadOnlyCollection<TestCollectionEntry> Attributes { get; private set; } 

	}

	public abstract class TestParentClass
	{

		

	}

	public class TestChildClass1 : TestParentClass {}

	public class TestChildClass2 : TestParentClass {}

}
