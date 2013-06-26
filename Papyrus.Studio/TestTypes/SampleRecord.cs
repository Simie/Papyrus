using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Papyrus.Core;
using Papyrus.Studio.Framework;

namespace Papyrus.Studio.TestTypes
{

	public class SampleRecord : Record
	{

		public string TestString { get; private set; }

		public int TestInteger { get; private set; }

	}

	public class SampleRecord2 : Record
	{

		public string TestierString { get; private set; }

		public int TestierInteger { get; private set; }

		public RecordRef<SampleRecord> TestReference { get; private set; } 

	}
	
	public class SampleRecord4 : Record
	{

		public RecordRefCollection<SampleRecord> TestReferences { get; private set; } 

	}

}
