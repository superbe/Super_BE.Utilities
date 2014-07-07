using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Super_BE.Utilities.Extensions;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Super_BE.Utilities.Tests
{
	[TestClass]
	public class ExtensionTests
	{
		[TestMethod]
		public void TestBinaryConvert()
		{
			// Создали тестовый объект класса TestsClass.
			TestsClass audited = new TestsClass { Id = 567, Name = "Проба пера" };
			// Преобразовали его в бинарник.
			byte[] transitional = audited.ToBinary();
			// Преобразовали бинарник в исходный объект.
			TestsClass result = transitional.FromBinary<TestsClass>();
			// Делаем проверки.
			Assert.AreEqual(audited.Id, result.Id);
			Assert.AreEqual(audited.Name, result.Name);
		}

		[TestMethod]
		public void TestJSFormat()
		{
			// Создали тестовый объект класса TestsClass.
			string audited = "<span class=\"reserved\">var</span> item = {\r\n    id:567,\r\n    name:&quot;Проба пера&quot;\r\n};";
			// Создали проверяемый объект.
			string tested = "var item = {id:567,name:\"Проба пера\"};";
			// Отформатировали исходную строку.
			string result = tested.JSFormat();
			// Проверили совпадение тестового и проверяемого объектов.
			Assert.AreEqual(audited, result);
		}

		[TestMethod]
		public void TestNull()
		{
			// Создали тестовый объект класса TestsClass.
			TestsClass tested = null;
			// Провериили, что он пустой.
			Assert.AreEqual(true, tested.IsNull());
			// Присвоили ему значение.
			tested = new TestsClass { Id = 567, Name = "Проба пера" };
			// Провериили, что он не пустой.
			Assert.AreEqual(false, tested.IsNull());
			// Объект вернул пустой объект.
			Assert.AreEqual(null, tested.ToNull());
		}

		[TestMethod]
		public void TestXMLConvert()
		{
			// Создали тестовый объект класса TestsClass.
			TestsClass audited = new TestsClass { Id = 567, Name = "Проба пера" };
			// Преобразовали его в XML-документ.
			XDocument transitional = audited.ToXML();
			// Преобразовали XML-документ в исходный объект.
			TestsClass result = transitional.FromXML<TestsClass>();
			// Делаем проверки.
			Assert.AreEqual(audited.Id, result.Id);
			Assert.AreEqual(audited.Name, result.Name);
		}

		[TestMethod]
		public void TestJSONConvert()
		{
			// Создали тестовый объект класса TestsClass.
			TestsClass audited = new TestsClass { Id = 567, Name = "Проба пера" };
			// Преобразовали его в XML-документ.
			string transitional = audited.ToJson();
			// Преобразовали XML-документ в исходный объект.
			TestsClass result = transitional.FromJson<TestsClass>();
			// Делаем проверки.
			Assert.AreEqual(audited.Id, result.Id);
			Assert.AreEqual(audited.Name, result.Name);
		}

		[TestMethod]
		public void TestConvert()
		{
			// Создали тестовый объект класса TestsClass.
			TestsClass audited = new TestsClass { Id = 567, Name = "Проба пера" };

			// Binary
			// Сохранить данные.
			audited.Save(@"C:\Temp\test.bin", TypeConvert.Binary);
			// Загрузить данные.
			TestsClass result = new TestsClass();
			result = (TestsClass)result.Load(@"C:\Temp\test.bin", TypeConvert.Binary);
			// Делаем проверки.
			Assert.AreEqual(audited.Id, result.Id);
			Assert.AreEqual(audited.Name, result.Name);

			// Json
			// Сохранить данные.
			audited.Save(@"C:\Temp\test.json", TypeConvert.Json);
			// Загрузить данные.
			result = new TestsClass();
			result = (TestsClass)result.Load(@"C:\Temp\test.json", TypeConvert.Json);
			// Делаем проверки.
			Assert.AreEqual(audited.Id, result.Id);
			Assert.AreEqual(audited.Name, result.Name);

			// Xml
			// Сохранить данные.
			audited.Save(@"C:\Temp\test.xml", TypeConvert.Xml);
			// Загрузить данные.
			result = new TestsClass();
			result = (TestsClass)result.Load(@"C:\Temp\test.xml", TypeConvert.Xml);
			// Делаем проверки.
			Assert.AreEqual(audited.Id, result.Id);
			Assert.AreEqual(audited.Name, result.Name);
		}
	}
}
