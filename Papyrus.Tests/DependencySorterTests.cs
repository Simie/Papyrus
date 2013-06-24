using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Papyrus.Core.Util;
namespace Papyrus.Tests
{
	[TestClass]
	public class DependencySorterTests
	{
		
		class Entry
		{

			public string Name;
			public List<Entry> Dependencies = new List<Entry>();

		}

		[TestMethod]
		public void TestSimpleTree()
		{

			var itemA = new Entry() {Name = "A"};
			var itemB = new Entry() {Name = "B", Dependencies = new List<Entry>() {itemA}};
			var itemC = new Entry() {Name = "C", Dependencies = new List<Entry>() {itemB}};

			var collection = new List<Entry>();
			collection.Add(itemB);
			collection.Add(itemC);
			collection.Add(itemA);

			var sorted = collection.TSort(entry => entry.Dependencies).ToList();

			Assert.IsTrue(sorted[0] == itemA);
			Assert.IsTrue(sorted[1] == itemB);
			Assert.IsTrue(sorted[2] == itemC);

		}

	}
}