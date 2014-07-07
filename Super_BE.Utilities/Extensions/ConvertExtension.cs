using System;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Super_BE.Utilities.Extensions
{
	/// <summary>
	/// Расширение преобразования объектов в бинарный, JSON и XML форматы.
	/// </summary>
	/// <example>
	/// // Создали тестовый объект класса TestsClass.
	/// TestsClass audited = new TestsClass { Id = 567, Name = "Проба пера" };
	///
	/// // Binary
	/// // Сохранить данные.
	/// audited.Save(@"C:\Temp\test.bin", TypeConvert.Binary);
	/// // Загрузить данные.
	/// TestsClass result = new TestsClass();
	/// result = (TestsClass)result.Load(@"C:\Temp\test.bin", TypeConvert.Binary);
	/// 
	/// // Json
	/// // Сохранить данные.
	/// audited.Save(@"C:\Temp\test.json", TypeConvert.Json);
	/// // Загрузить данные.
	/// result = new TestsClass();
	/// result = (TestsClass)result.Load(@"C:\Temp\test.json", TypeConvert.Json);
	///
	/// // Xml
	/// // Сохранить данные.
	/// audited.Save(@"C:\Temp\test.xml", TypeConvert.Xml);
	/// // Загрузить данные.
	/// result = new TestsClass();
	/// result = (TestsClass)result.Load(@"C:\Temp\test.xml", TypeConvert.Xml);
	/// </example>
	public static class ConvertExtension
	{
		/// <summary>
		/// Сохранить объект в один из форматов: бинарный, JSON или XML.
		/// </summary>
		/// <typeparam name="T">Тип объекта.</typeparam>
		/// <param name="instance">Экземпляр объекта.</param>
		/// <param name="fileName">Имя файла.</param>
		/// <param name="format">Тип формата.</param>
		/// <param name="anyway">Конвертировать любым образом.</param>
		/// <example>
		/// // Создали тестовый объект класса TestsClass.
		/// TestsClass audited = new TestsClass { Id = 567, Name = "Проба пера" };
		/// // Сохранить данные.
		/// audited.Save(@"C:\Temp\test.bin", TypeConvert.Binary);
		/// </example>
		public static void Save<T>(this T instance, string fileName, TypeConvert format = TypeConvert.Default, bool anyway = false)
		{
			try
			{
				switch (format)
				{
					case TypeConvert.Binary:
						File.WriteAllBytes(fileName, instance.ToBinary());
						break;
					case TypeConvert.Json:
						File.WriteAllText(fileName, instance.ToJson(anyway));
						break;
					case TypeConvert.Xml:
						instance.ToXML().Save(fileName);
						break;
					default:
						File.WriteAllText(fileName, instance.ToString());
						break;
				}
			}
			catch (Exception)
			{

			}
		}

		/// <summary>
		/// Загрузить объект из бинарного, JSON или XML формата.
		/// </summary>
		/// <typeparam name="T">Тип объекта.</typeparam>
		/// <param name="instance">Экземпляр объекта.</param>
		/// <param name="fileName">Имя файла.</param>
		/// <param name="format">Тип формата.</param>
		/// <returns>Загруженный объект.</returns>
		/// <example>
		/// // Загрузить данные.
		/// result = (TestsClass)result.Load(@"C:\Temp\test.bin", TypeConvert.Binary);
		/// </example>
		public static object Load<T>(this T instance, string fileName, TypeConvert format = TypeConvert.Default)
		{
			try
			{
				switch (format)
				{
					case TypeConvert.Binary:
						return File.ReadAllBytes(fileName).FromBinary<T>();
					case TypeConvert.Json:
						return File.ReadAllText(fileName).FromJson<T>();
					case TypeConvert.Xml:
						return XDocument.Load(fileName).FromXML<T>();
					default:
						return File.ReadAllText(fileName);
				}
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}