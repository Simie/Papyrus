using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Papyrus.Tests
{
	[TestClass]
	public class RecordTests
	{

		[TestMethod]
		public void TestFreeze()
		{

			var record = new TestRecord();

			record.IsFrozen = false;

			try {
				record.SetProperty(() => record.TestBoolean, true);
			} catch (InvalidOperationException) {
				Assert.Fail("Threw exception when record was not frozen.");
			}

			record.IsFrozen = true;
	
			try {
				record.SetProperty(() => record.TestBoolean, true);
				Assert.Fail("Didn't throw exception when record was frozen.");
			} catch (InvalidOperationException) {}

		}

	}
}
