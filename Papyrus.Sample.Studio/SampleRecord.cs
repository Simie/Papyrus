/*
 * Copyright © 2013 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Papyrus.Core;

namespace Papyrus.Studio.TestTypes
{

	public class TestStuff : IEquatable<TestStuff>
	{

		public string Property { get; set; }

		public int Property2 { get; set; }

		public override string ToString()
		{
			return Property ?? "No Value";
		}

		public bool Equals(TestStuff other)
		{
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;
			return string.Equals(Property, other.Property) && Property2 == other.Property2;
		}
		
	}

	public abstract class TestBase { }
	public class Test1 : TestBase { }
	public class Test2 : TestBase { }

	public class SampleRecord : Record
	{

		private FlagsTest _enumFlags = FlagsTest.Four;

		[Flags]
		public enum FlagsTest
		{

			None = 0,
			One = 1 << 0,
			Two = 1 << 1,
			Three = 1 << 2,
			Four = 1 << 3,

			All = One | Two | Three | Four

		}

		[PropertyTools.DataAnnotations.Comment]
		public string ShouldIgnore {get { return "ShouldIgnore!"; }}

		public string TestString { get; set; }

		[PropertyTools.DataAnnotations.Spinnable]
		public int TestInteger { get; set; }

		[PropertyTools.DataAnnotations.Spinnable]
		public float TestFloat { get; set; }

		public RecordRef<ParentRecord> TestPolyRef { get; set; }

		public ReadOnlyCollection<TestStuff> TestCollection { get; set; }

		public ReadOnlyCollection<string> PrimitiveCollectionTest { get; set; } 

		public FlagsTest EnumFlags
		{
			get { return _enumFlags; }
			set { _enumFlags = value; }
		}

	}

	public class SampleRecord2 : Record
	{

		public string TestierString { get; set; }

		public int TestierInteger { get; set; }

		public RecordRef<SampleRecord> TestReference { get; set; } 

	}
	
	public class SampleRecord4 : Record
	{

		public RecordRefCollection<SampleRecord> TestReferences { get; set; } 

	}

	public abstract class ParentRecord : Record
	{

		public string ParentProperty { get; set; }

	}

	public class ChildRecord1 : ParentRecord
	{

		public bool ChildProperty1 { get; set; }

	}
	
	public class ChildRecord2 : ParentRecord
	{

		public int ChildProperty2 { get; set; }

	}
	
	public class ChildRecord3 : ParentRecord
	{

		public string ChildProperty3 { get; set; }

	}

}
