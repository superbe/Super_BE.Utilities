using System.Xml.Linq;
using System.Xml.Serialization;

namespace Super_BE.Utilities.Extensions
{
	/// <summary>
	/// Расширение сериализации в XML-документ.
	/// </summary>
	/// <example>
	/// // Создали тестовый объект класса TestsClass.
	/// TestsClass instance = new TestsClass { Id = 567, Name = "Проба пера" };
	/// // Преобразовали его в XML-документ.
	/// XDocument transitional = audited.ToXML();
	/// // Преобразовали XML-документ в исходный объект.
	/// TestsClass result = transitional.FromXML<TestsClass>();
	/// </example>
	/// <remarks>
	/// Внимание! Важное условие: класс должен быть сериализуемым. т.е. Классу должен быть задан атрибут [Serializable].
	/// </remarks>
	public static class XMLConvertExtension
	{
		/// <summary>
		/// Конвертировать экземпляр объекта в XML-документ.
		/// </summary>
		/// <typeparam name="T">Тип объекта.</typeparam>
		/// <param name="instance">Экземпляр объекта.</param>
		/// <returns>Экземпляр объекта в представлении XML-документа.</returns>
		/// <example>
		/// TestsClass instance = new TestsClass { Id = 567, Name = "Проба пера" };
		/// XDocument transitional = audited.ToXML();
		/// </example>
		/// <remarks>
		/// Внимание! Важное условие: класс должен быть сериализуемым. т.е. Классу должен быть задан атрибут [Serializable].
		/// </remarks>
		public static XDocument ToXML<T>(this T instance)
		{
			var document = new XDocument();
			using (var writer = document.CreateWriter())
			{
				var serializer = new XmlSerializer(instance.GetType());
				serializer.Serialize(writer, instance);
			}
			return document;
		}

		/// <summary>
		/// Конвертировать XML-документ в экземпляр объекта.
		/// </summary>
		/// <typeparam name="T">Тип объекта.</typeparam>
		/// <param name="document">XML-документ.</param>
		/// <returns>Экземпляр объекта.</returns>
		/// <example>
		/// TestsClass result = transitional.FromXML<TestsClass>();
		/// </example>
		/// <remarks>
		/// Внимание! Важное условие: класс должен быть сериализуемым. т.е. Классу должен быть задан атрибут [Serializable].
		/// </remarks>
		public static T FromXML<T>(this XDocument document)
		{
			var serializer = new XmlSerializer(typeof(T));
			if (document.Root != null)
				using (var reader = document.Root.CreateReader())
				{
					return (T)serializer.Deserialize(reader);
				}
			return default(T);
		}
	}
}