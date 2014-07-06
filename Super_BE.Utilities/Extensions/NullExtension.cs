using System;

namespace Super_BE.Utilities.Extensions
{
	/// <summary>
	/// Расширение проверки объекта на null.
	/// </summary>
	public static class NullExtension
	{
		/// <summary>
		/// Проверить объект на null.
		/// </summary>
		/// <typeparam name="T">Тип объекта.</typeparam>
		/// <param name="instance">Экземпляр объекта.</param>
		/// <returns>Результат проверки: true - объект пуст, в противном случае false.</returns>
		/// <example>
		/// if (queryStrings.IsNull())
		/// 	return string.Empty;
		/// </example>
		public static bool IsNull<T>(this T instance)
		{
			// ReSharper disable CompareNonConstrainedGenericWithNull
			return instance == null || ReferenceEquals(instance, DBNull.Value) || Convert.IsDBNull(instance);
			// ReSharper restore CompareNonConstrainedGenericWithNull
		}

		/// <summary>
		/// Вернуть нулевой объект.
		/// </summary>
		/// <typeparam name="T">Тип объекта.</typeparam>
		/// <param name="instance">Экземпляр объекта.</param>
		/// <returns>Возвращаемое значение.</returns>
		/// <example>
		/// var query = queryStrings.ToNull();
		/// </example>
		public static object ToNull<T>(this T instance)
		{
			return null;
		}
	}
}