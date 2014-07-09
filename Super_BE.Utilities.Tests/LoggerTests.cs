using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Super_BE.Utilities.Diagnostics;
using System.IO;
using System.Web;
using System.Web.SessionState;
using Super_BE.Utilities.Extensions;

namespace Super_BE.Utilities.Tests
{
	[TestClass]
	public class LoggerTests
	{
		private Logger _log;

		public string Folder { get { return (new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location)).DirectoryName; } }

		public string PathInformation { get { return Path.Combine(Folder, "Logs/Informations/"); } }

		public string PathWarning { get { return Path.Combine(Folder, "Logs/Warning/"); } }

		public string PathTrace { get { return Path.Combine(Folder, "Logs/Trace/"); } }

		public string PathHttp { get { return Path.Combine(Folder, "Logs/Http/"); } }

		public string PathErrors { get { return Path.Combine(Folder, "Logs/Errors/"); } }

		public LoggerTests()
		{
			_log = Logger.Implementation();
		}

		private DirectoryInfo GetDirectory(string directoryName)
		{
			DirectoryInfo directory = new DirectoryInfo(directoryName);
			if (directory.Exists)
			{
				FileInfo[] files = directory.GetFiles();
				foreach (FileInfo file in files) file.Delete();
			}
			return directory;
		}

		public static HttpContext FakeHttpContext(string url)
		{
			var uri = new Uri(url);
			var httpRequest = new HttpRequest(string.Empty, uri.ToString(), uri.Query.TrimStart('?'));
			var stringWriter = new StringWriter();
			var httpResponse = new HttpResponse(stringWriter);
			var httpContext = new HttpContext(httpRequest, httpResponse);
			var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
													new HttpStaticObjectsCollection(), 10, true,
													HttpCookieMode.AutoDetect,
													SessionStateMode.InProc, false);
			SessionStateUtility.AddHttpSessionStateToContext(httpContext, sessionContainer);
			return httpContext;
		}

		[TestMethod]
		public void TestInformation()
		{
			DirectoryInfo directory = GetDirectory(PathInformation);
			_log.Information("Проба {0} из {1} {0}", "пера", "гусиного");
			Assert.AreEqual("Проба пера из гусиного пера", File.ReadAllText(directory.GetFiles()[0].FullName));
		}

		[TestMethod]
		public void TestWarning()
		{
			DirectoryInfo directory = GetDirectory(PathWarning);
			_log.Warning("Проба {0} из {1} {0}", "пера", "гусиного");
			Assert.AreEqual("Проба пера из гусиного пера", File.ReadAllText(directory.GetFiles()[0].FullName));
		}

		[TestMethod]
		public void TestTraceApi()
		{
			DirectoryInfo directory = GetDirectory(PathTrace);
			_log.TraceApi("1", "2", new TimeSpan(2, 14, 18));
			Assert.AreEqual("Component:1;\r\nMethod:2;\r\nTimespan:02:14:18;\r\nProperties:", File.ReadAllText(directory.GetFiles()[0].FullName));
			directory = GetDirectory(PathTrace);
			_log.TraceApi("1", "2", new TimeSpan(2, 14, 18), "3");
			Assert.AreEqual("Component:1;\r\nMethod:2;\r\nTimespan:02:14:18;\r\nProperties:3", File.ReadAllText(directory.GetFiles()[0].FullName));
			directory = GetDirectory(PathTrace);
			_log.TraceApi("1", "2", new TimeSpan(2, 14, 18), "3 {0}", 4);
			Assert.AreEqual("Component:1;\r\nMethod:2;\r\nTimespan:02:14:18;\r\nProperties:3 4", File.ReadAllText(directory.GetFiles()[0].FullName));
		}

		[TestMethod]
		public void TestHttp()
		{
			DirectoryInfo directory = GetDirectory(PathHttp);
			HttpRequest request = FakeHttpContext("http://www.google.com/?name=gdfgd").Request;
			_log.Http(request, 2);
			HttpDataItem httpDataItem = new HttpDataItem();
			httpDataItem = (HttpDataItem)httpDataItem.Load(directory.GetFiles()[0].FullName, TypeConvert.Json);
			Assert.AreEqual("GET", httpDataItem.HttpMethod);
			Assert.AreEqual("http://www.google.com/?name=gdfgd", httpDataItem.Url.AbsoluteUri);
		}

		[TestMethod]
		public void TestError()
		{
			try
			{
				throw new Exception("Test");
			}
			catch (Exception exception)
			{
				DirectoryInfo directory = GetDirectory(PathErrors);
				_log.Error(exception);
				Assert.AreEqual(1, directory.GetFiles().Length);
			}
		}
	}
}
