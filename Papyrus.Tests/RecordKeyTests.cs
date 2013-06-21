using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Papyrus.Core;

namespace Papyrus.Tests
{
	[TestClass]
	public class RecordKeyTests
	{
		[TestMethod]
		public void TestStringRep()
		{
			
			// Test odd plugin name
			const string testKey1 = "Some-Plugin'Name/00032F";

			// Test simple key
			const string testKey2 = "Plugin/FFFFFF";

			// Test root key (no plugin name)
			const string testKey3 = "DFFFFF";


			var key1 = RecordKey.FromString(testKey1);
			Assert.AreEqual(key1.Plugin, "Some-Plugin'Name");
			Assert.AreEqual(key1.Index, 815);

			var key2 = RecordKey.FromString(testKey2);
			Assert.AreEqual(key2.Plugin, "Plugin");
			Assert.AreEqual(key2.Index, 16777215);

			var key3 = RecordKey.FromString(testKey3);
			Assert.IsNull(key3.Plugin);
			Assert.AreEqual(key3.Index, 14680063);

			// check that constructor yields the same result as FromString
			var tKey1 = RecordKey.FromString(testKey2);
			var tKey2 = new RecordKey(testKey2);

			Assert.AreEqual(tKey1, tKey2);

		}

		[TestMethod]
		public void TestInvalidKey()
		{

			const string invalidKey1 = "Some/Plugin/000000";
			const string invalidKey2 = "SomePlugin000000";

			try {
				RecordKey.FromString(invalidKey1);
				Assert.Fail("Invalid key didn't throw exception");
			} catch(FormatException) {} 
	
			try {
				RecordKey.FromString(invalidKey2);
				Assert.Fail("Invalid key didn't throw exception");
			} catch(FormatException) {} 

		}

	}
}
