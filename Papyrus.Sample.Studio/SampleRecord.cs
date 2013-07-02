/*
 * Copyright © 2013 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using Papyrus.Core;

namespace Papyrus.Studio.TestTypes
{

	public class SampleRecord : Record
	{

		public string TestString { get; private set; }

		public int TestInteger { get; private set; }

		public RecordRef<ParentRecord> TestPolyRef { get; private set; }

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

	public abstract class ParentRecord : Record
	{

		public string ParentProperty { get; private set; }

	}

	public class ChildRecord1 : ParentRecord
	{

		public bool ChildProperty1 { get; private set; }

	}
	
	public class ChildRecord2 : ParentRecord
	{

		public int ChildProperty2 { get; private set; }

	}
	
	public class ChildRecord3 : ParentRecord
	{

		public string ChildProperty3 { get; private set; }

	}

}
