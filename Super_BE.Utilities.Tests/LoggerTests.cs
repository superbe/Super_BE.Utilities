using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Super_BE.Utilities.Diagnostics;

namespace Super_BE.Utilities.Tests
{
	[TestClass]
	public class LoggerTests
	{
		private Logger _log;

		public LoggerTests()
		{
			_log = Logger.Implementation();
		}

		[TestMethod]
		public void TestMethod1()
		{
			_log.Warning("Test");
			//Assert.AreEqual("Test", )
		}
	}
}
