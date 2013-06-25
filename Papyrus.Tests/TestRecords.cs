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

}
