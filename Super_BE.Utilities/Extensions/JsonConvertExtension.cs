using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

namespace Super_BE.Utilities.Extensions
{
	/// <summary>
	/// Дополнение класса System.String.
	/// </summary>
	/// TODO: Переписать с учетом классов парсинга JS-скриптов.
	public static class JsonConvertExtension
	{
		/// <summary>
		/// Конвертировать экземпляр объекта в Json-строку.
		/// </summary>
		/// <typeparam name="T">Тип объекта.</typeparam>
		/// <param name="instance">Экземпляр объекта.</param>
		/// <param name="anyway">Строгость обработки.</param>
		/// <returns>Экземпляр объекта в представлении Json-строки.</returns>
		public static string ToJson<T>(this T instance, bool anyway = false)
		{
			try
			{
				var memoryStream = new MemoryStream();
				var jsonSerializer = new DataContractJsonSerializer(instance.GetType());
				jsonSerializer.WriteObject(memoryStream, instance);
				byte[] json = memoryStream.ToArray();
				memoryStream.Close();
				return Encoding.UTF8.GetString(json, 0, json.Length);
			}
			catch (Exception)
			{
				// Если экземпляр объекта не сериализуется.
				// Обработать запрос надо в любом случае.
				if (anyway)
				{
					return GetJson(instance, new Stack());
				}
				throw;
			}
		}

		/// <summary>
		/// Конвертировать нессериализуемый экземпляр объекта в Json-строку.
		/// </summary>
		/// <typeparam name="T">Тип объекта.</typeparam>
		/// <param name="instance">Экземпляр объекта.</param>
		/// <param name="stack">Стек проверки циклических ссылочных объектов.</param>
		/// <returns>Экземпляр объекта в представлении Json-строки.</returns>
		private static string GetJson<T>(T instance, Stack stack)
		{
			if (instance.IsNull()) return "null";
			Type instanceType = instance.GetType();
			if (stack.Contains(instance.GetHashCode())) return "{ \"COPY\": { } }";
			stack.Push(instance.GetHashCode());
			var result = new List<string>();
			try
			{
				foreach (MemberInfo memberInfo in instanceType.GetMembers())
				{
					// Поле.
					if (memberInfo.MemberType == MemberTypes.Field)
					{
						var field = (FieldInfo)memberInfo;
						if (field.IsPublic)
						{
							var fieldInstance = GetFieldInstance(instance, field);
							if (!fieldInstance.IsNull() && !stack.Contains(fieldInstance.GetHashCode()))
							{
								InsertValue(field.Name, GetValue(fieldInstance, stack), result);
							}
						}
					}
					// Свойство.
					if (memberInfo.MemberType == MemberTypes.Property)
					{
						var property = (PropertyInfo)memberInfo;
						if (property.GetGetMethod(false).GetParameters().Length == 0 && property.CanRead && !property.GetGetMethod(false).IsAbstract)
						{
							var propertyInstance = GetPropertyInstance(instance, property);
							if (!propertyInstance.IsNull() && !stack.Contains(propertyInstance.GetHashCode()))
							{
								InsertValue(property.Name, GetValue(propertyInstance, stack), result);
							}
						}
					}
				}
				return string.Format("{{ {0} }}", string.Join(",", result.ToArray()));
			}
			catch (Exception exception)
			{
				// На тот случай если какой-то метод работает с ошибкой или недоступен.
				return string.Format("{{ \"ERROR\": \"{0}\" }}", HttpUtility.JavaScriptStringEncode(exception.ToString()));
			}
		}

		/// <summary>
		/// Получить данные поля.
		/// </summary>
		/// <typeparam name="T">Тип поля.</typeparam>
		/// <param name="instance">Экземпляр объекта.</param>
		/// <param name="field">Метаданные поля.</param>
		/// <returns>Значение экземпляра поля.</returns>
		private static object GetFieldInstance<T>(T instance, FieldInfo field)
		{
			try
			{
				return field.GetValue(instance);
			}
			catch (Exception exception)
			{
				return CheckException(exception) ? null : string.Format("{{ \"ERROR\": \"{0}\" }}", HttpUtility.JavaScriptStringEncode(exception.ToString()));
			}
		}

		/// <summary>
		/// Получить данные свойства.
		/// </summary>
		/// <typeparam name="T">Тип свойства.</typeparam>
		/// <param name="instance">Экземпляр объекта.</param>
		/// <param name="property">Метаданные свойства.</param>
		/// <returns>Значение экземпляра свойства.</returns>
		private static object GetPropertyInstance<T>(T instance, PropertyInfo property)
		{
			try
			{
				return property.GetValue(instance, new object[] { });
			}
			catch (Exception exception)
			{
				return CheckException(exception) ? null : string.Format("{{ \"ERROR\": \"{0}\" }}", HttpUtility.JavaScriptStringEncode(exception.ToString()));
			}
		}

		/// <summary>
		/// Проверить возбужденное исключение на исключения возбуждаемые для абстрактных методов.
		/// </summary>
		/// <param name="exception">Исключение.</param>
		/// <returns>Результат проверки: true - возвращаемый метод либо абстрактный, либо имеет недопустимые аргументы, в противном случае метод разрешен.</returns>
		private static bool CheckException(Exception exception)
		{
			return exception.InnerException.GetType() == typeof(InvalidOperationException) || exception.InnerException.GetType() == typeof(NotSupportedException) || exception.InnerException.GetType() == typeof(ArgumentNullException);
		}

		/// <summary>
		/// Вставить не пустое значение.
		/// </summary>
		/// <param name="name">Наименование свойства или поля.</param>
		/// <param name="value">Значение свойства или поля.</param>
		/// <param name="result">Сформированная Json-строка.</param>
		private static void InsertValue(string name, string value, List<string> result)
		{
			if (!value.IsNull())
				result.Add(string.Format("\"{0}\": {1}", name, value));
		}

		/// <summary>
		/// Получить преобразованное свойство.
		/// </summary>
		/// <param name="value">Значение свойства объекта.</param>
		/// <param name="stack">Стек проверки циклических ссылочных объектов.</param>
		/// <returns>Преобразованное свойство.</returns>
		private static string GetValue<T>(T value, Stack stack)
		{
			if (value.IsNull()) return "null";
			if (value is DateTime) return GetDateTime(value);
			if (value is bool) return GetLogical(value);
			if (value is Color) return GetColor(value);
			if (value is IntPtr) return GetIntPtr(value);
			var type = value as Type;
			if (type != null) return GetLiteral(type.Name);
			if (CheckNumericalType(value)) return GetNumerical(value);
			if (value.GetType().BaseType == typeof(Enum)) return GetEnumValue(value.GetType(), value);
			if (value.GetType().IsArray) return GetArray(value, stack);
			if (!(value as string).IsNull()) return GetLiteral(value as string);
			var assembly = value as Assembly;
			if (assembly != null) return string.Format("\"{0}\"", assembly.FullName);
			var module = value as Module;
			if (module != null) return string.Format("\"{0}\"", module.Name);
			return GetJson(value, stack);
		}

		/// <summary>
		/// Преобразовать перечисление в Json-представление.
		/// </summary>
		/// <param name="type">Тип перечисления.</param>
		/// <param name="value">Перечисление.</param>
		/// <returns>Перечисление в Json-представление.</returns>
		/// <remarks>Возможно надо будет возвращать в числовом виде.</remarks>
		private static string GetEnumValue(Type type, object value)
		{
			var result = new List<string>();
			if (type.IsEnum)
			{
				FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
				result.AddRange(from t in fields let fieldvalue = (Int32)t.GetValue(null) where (fieldvalue & (int)value) == fieldvalue select t.Name);
			}
			return string.Format("\"{0}\"", string.Join("|", result.ToArray()));
		}

		/// <summary>
		/// Преобразовать значение типа IntPtr в Json-представление.
		/// </summary>
		/// <param name="value">Значение типа IntPtr.</param>
		/// <returns>Значение типа IntPtr в Json-представлении.</returns>
		private static string GetIntPtr(object value)
		{
			return ((IntPtr)value).ToString();
		}

		/// <summary>
		/// Преобразовать значение типа DateTime в Json-представление.
		/// </summary>
		/// <param name="value">Значение типа DateTime.</param>
		/// <returns>Значение типа DateTime в Json-представлении.</returns>
		private static string GetDateTime(object value)
		{
			return string.Format("\"{0}\"", ((DateTime)value).ToString(CultureInfo.CurrentCulture));
		}

		/// <summary>
		/// Преобразовать логическое значение в Json-представление.
		/// </summary>
		/// <param name="value">Логическое значение.</param>
		/// <returns>Логическое значение. в Json-представление.</returns>
		internal static string GetLogical(object value)
		{
			return ((bool)value).ToString(CultureInfo.InvariantCulture).ToLower();
		}

		/// <summary>
		/// Преобразовать цвет в Json-представление.
		/// </summary>
		/// <param name="value">Цвет.</param>
		/// <returns>Цвет в Json-представление.</returns>
		private static string GetColor(object value)
		{
			return (Color)value == default(Color)
					   ? string.Empty
					   : string.Format("\"{0}\"",
									   ((Color)value).A != 0 && ((Color)value).A != 255
										   ? string.Format("rgba({0}, {1}, {2}, {3})", ((Color)value).R, ((Color)value).G,
														   ((Color)value).B,
														   ((decimal)((Color)value).A / 255).ToString(CultureInfo.InvariantCulture))
										   : ColorTranslator.ToHtml((Color)value));
		}

		/// <summary>
		/// Преобразовать число в Json-представление.
		/// </summary>
		/// <param name="value">Число.</param>
		/// <returns>Число в Json-представление.</returns>
		private static string GetNumerical<T>(T value)
		{
			return string.Format("{0}", Convert.ToString(value, CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Преобразовать массив в Json-представление.
		/// </summary>
		/// <param name="value">Массив.</param>
		/// <param name="stack">Стек проверки циклических ссылочных объектов.</param>
		/// <returns>Массив в Json-представление.</returns>
		private static string GetArray(object value, Stack stack)
		{
			var array = ToArray(value);
			return string.Format("[{0}]", string.Join(", ", array.Select(item => GetValue(item, stack)).ToArray()));
		}

		/// <summary>
		/// Преобразовать массив элементов произвольного типа в массив объектов.
		/// </summary>
		/// <param name="value">Массив.</param>
		/// <returns>Массив объектов.</returns>
		private static IEnumerable<object> ToArray(object value)
		{
			var objects = value as IEnumerable;
			if (objects != null)
				return objects.Cast<object>().ToArray();
			return new[] { value };
		}

		/// <summary>
		/// Преобразовать строку в Json-представление.
		/// </summary>
		/// <param name="value">Строка.</param>
		/// <returns>Строка в Json-представление.</returns>
		private static string GetLiteral(string value)
		{
			return string.IsNullOrWhiteSpace(value) ? null : string.Format("\"{0}\"", HttpUtility.JavaScriptStringEncode(value));
		}

		/// <summary>
		/// Проверить, что данный объект является числом.
		/// </summary>
		/// <param name="o">Проверяемый объект.</param>
		/// <returns>Результат провеки.</returns>
		private static bool CheckNumericalType(object o)
		{
			return o is byte || o is sbyte || o is decimal || o is double || o is float || o is int || o is uint || o is long || o is ulong || o is short || o is ushort;
		}

		/// <summary>
		/// Конвертировать Json-строку в экземпляр объекта.
		/// </summary>
		/// <param name="json">Json-строка.</param>
		/// <returns>Экземпляр объекта.</returns>
		public static T FromJson<T>(this string json)
		{
			var memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(json));
			var jsonSerializer = new DataContractJsonSerializer(typeof(T));
			var result = (T)jsonSerializer.ReadObject(memoryStream);
			memoryStream.Close();
			return result;
		}
	}
}