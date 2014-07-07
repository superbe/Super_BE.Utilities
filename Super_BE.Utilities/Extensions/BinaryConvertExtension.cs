using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Super_BE.Utilities.Diagnostics;

namespace Super_BE.Utilities.Extensions
{
	/// <summary>
	/// Расширение сериализации в бинарный код.
	/// </summary>
	/// <remarks>
	/// Внимание! Важное условие: класс должен быть сериализуемым. т.е. Классу должен быть задан атрибут [Serializable].
	/// </remarks>
	public static class BinaryConvertExtension
	{
		/// <summary>
		/// Конвертировать экземпляр объекта в байтовый массив.
		/// </summary>
		/// <typeparam name="T">Тип объекта.</typeparam>
		/// <param name="instance">Экземпляр объекта.</param>
		/// <returns>Экземпляр объекта в представлении байтового массива.</returns>
		/// <example>
		/// TestsClass instance = new TestsClass { Id = 567, Name = "Проба пера" };
		/// File.WriteAllBytes(fileName, instance.ToBinary());
		/// </example>
		public static byte[] ToBinary<T>(this T instance)
		{
			IFormatter formatter = new BinaryFormatter();
			using (var stream = new MemoryStream())
			{
				byte[] result;
				try
				{
					formatter.Serialize(stream, instance);
					result = stream.GetBuffer();
				}
				catch (Exception exception)
				{
					Logger.Implementation().Error(exception, string.Empty);
					throw;
				}
				finally
				{
					stream.Close();
				}
				return result;
			}
		}

		/// <summary>
		/// Конвертировать байтовый массив в экземпляр объекта.
		/// </summary>
		/// <typeparam name="T">Тип объекта.</typeparam>
		/// <param name="data">Байтовый массив</param>
		/// <returns>Экземпляр объекта.</returns>
		/// <example>
		/// TestsClass result = File.ReadAllBytes(fileName).FromBinary<TestsClass>();
		/// </example>
		/// <remarks>
		/// Внимание! Важное условие: класс должен быть сериализуемым. т.е. Классу должен быть задан атрибут [Serializable].
		/// </remarks>
		public static T FromBinary<T>(this byte[] data)
		{
			IFormatter formatter = new BinaryFormatter();
			using (var stream = new MemoryStream(data))
			{
				T result;
				try
				{
					result = (T) formatter.Deserialize(stream);
				}
				catch (Exception exception)
				{
					Logger.Implementation().Error(exception, string.Empty);
					throw;
				}
				finally
				{
					stream.Close();
				}
				return result;
			}
		}
	}
}