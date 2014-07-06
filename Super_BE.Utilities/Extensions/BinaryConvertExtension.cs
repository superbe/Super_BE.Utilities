using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Super_BE.Utilities.Diagnostics;

namespace Super_BE.Utilities.Extensions
{
	/// <summary>
	/// 
	/// </summary>
	public static class BinaryConvertExtension
	{
		/// <summary>
		/// Конвертировать экземпляр объекта в байтовый массив.
		/// </summary>
		/// <typeparam name="T">Тип объекта.</typeparam>
		/// <param name="instance">Экземпляр объекта.</param>
		/// <returns>Экземпляр объекта в представлении байтового массива.</returns>
		/// <example>
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
		public static T FromBinary<T>(this byte[] data)
		{
			IFormatter formatter = new BinaryFormatter();
			using (var stream = new MemoryStream())
			{
				T result;
				try
				{
					foreach (byte b in data) stream.WriteByte(b);
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