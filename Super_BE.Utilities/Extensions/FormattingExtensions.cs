using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Super_BE.Utilities.Extensions
{
	/// <summary>
	/// Класс форматирования JavaScript-объекта в строковом представлении в представлении отображения кода Json-строки для отображения на html-страничке.
	/// </summary>
	public static class FormattingExtensions
	{
		/// <summary>
		/// Отформатировать
		/// </summary>
		/// <param name="instance">Экземпляр строки.</param>
		/// <param name="indentSize">Величина отступа.</param>
		/// <param name="indented">Добавлять отступы.</param>
		/// <param name="toEncode">Кодировать спецсимволы для html-представления.</param>
		/// <param name="toHighlight">Выделять ключевые слова JavaScript.</param>
		/// <param name="transferArrays">Переносить в массивах.</param>
		/// <param name="transferObjects">Переносить объекты.</param>
		/// <returns>Отформатированнная строка.</returns>
		/// <example>
		/// string tested = "var item = {id:567,name:\"Проба пера\"};";
		/// string result = tested.JSFormat();
		/// </example>
		public static string JSFormat(this string instance, int indentSize = 4, bool indented = true, bool toEncode = true, bool toHighlight = true, bool transferArrays = false, bool transferObjects = false)
		{
			bool isArray = false;
			bool isString = false;
			char newlineCharacter = new char();
			int level = 0;
			char[] specialСharacters = { '{', '}', ',', '[', ']', '(', ')', '\\', '\'', '"' };
			if (string.IsNullOrEmpty(instance)) return string.Empty;
			string result = null;
			for (int index = 0; index < instance.Length; index++)
			{
				char c = instance[index];
				if (specialСharacters.Any(x => x == c))
				{
					if (isString)
					{
						if (index >= 0 && (newlineCharacter == c && instance[index - 1] != '\\')) isString = false;
						result += c;
					}
					else
					{
						if (c == '\'' || c == '"')
						{
							isString = true;
							newlineCharacter = c;
							result += c;
						}
						else
						{
							string subString;
							switch (c)
							{
								case '{':
									result += c;
									level++;
									subString = (instance[index + 1] == '}' || (instance[index + 1] == ' ' && instance[index + 2] == '}') || (instance[index + 1] == ' ' && instance[index + 2] == ' ' && instance[index + 3] == '}')) ? string.Empty : InsertIdentNr(level, indented, indentSize);
									if (!string.IsNullOrEmpty(InsertIdentNr(level, indented, indentSize)) && index < instance.Length && instance[index + 1] == ' ') index++;
									result += subString;
									break;
								case '}':
									level--;
									result += (instance[index - 1] == '{' || (instance[index - 1] == ' ' && instance[index - 2] == '{') || (instance[index - 1] == ' ' && instance[index - 2] == ' ' && instance[index - 3] == '{')) ? string.Empty : InsertIdentNr(level, indented, indentSize);
									result += c;
									break;
								case '[':
									result += c;
									isArray = true;
									if (transferArrays)
									{
										level++;
										subString = (instance[index + 1] == ']' || (instance[index + 1] == ' ' && instance[index + 2] == ']') || (instance[index + 1] == ' ' && instance[index + 2] == ' ' && instance[index + 3] == ']')) ? string.Empty : InsertIdentNr(level, indented, indentSize);
										if (!string.IsNullOrEmpty(InsertIdentNr(level, indented, indentSize)) && index < instance.Length && instance[index + 1] == ' ') index++;
										result += subString;
									}
									break;
								case ']':
									if (transferArrays)
									{
										level--;
										result += (instance[index - 1] == '[' || (instance[index - 1] == ' ' && instance[index - 2] == '[') || (instance[index - 1] == ' ' && instance[index - 2] == ' ' && instance[index - 3] == '[')) ? string.Empty : InsertIdentNr(level, indented, indentSize);
									}
									result += c;
									isArray = false;
									break;
								case ',':
									result += c;
									subString = InsertIdentNr(instance, index, level, indented, indentSize, isArray, transferObjects, transferArrays);
									if (!string.IsNullOrEmpty(subString) && index < instance.Length && instance[index + 1] == ' ') index++;
									result += subString;
									break;
								default:
									result += c;
									break;
							}
						}
					}
				}
				else
				{
					result += c;
				}
			}
			return toHighlight ? HighlightKeywords(toEncode ? HttpUtility.HtmlEncode(result) : result) : toEncode ? HttpUtility.HtmlEncode(result) : result;
		}

		/// <summary>
		/// Сформировать отступ.
		/// </summary>
		/// <param name="level">Величина отступа.</param>
		/// <returns>Подстрока отступа.</returns>
		private static string Indent(int level, bool indented, int indentSize)
		{
			if (!indented) return string.Empty;
			string result = null;
			int length = level * indentSize;
			for (int i = 0; i < length; i++) result += " ";
			return result;
		}

		/// <summary>
		/// Сформировать подстроку вставки. Первая перегрузка.
		/// </summary>
		/// <param name="source">Исходная строка форматирования.</param>
		/// <param name="index">Индекс специального знака в строке.</param>
		/// <returns>Подстрока вставки.</returns>
		private static string InsertIdentNr(string source, int index, int level, bool indented, int indentSize, bool isArray, bool transferObjects, bool transferArrays)
		{
			if (!transferArrays && isArray) return string.Empty;
			if (source[index - 1] == '"' && source[index - 2] == '\\' && source[index - 3] == '\\') return InsertIdentNr(level, indented, indentSize);
			if (!transferObjects && index >= 0 && source[index - 1] == '}' && source[index + 1] == ' ' && source[index + 2] == '{') return string.Empty;
			return InsertIdentNr(level, indented, indentSize);
		}

		/// <summary>
		/// Сформировать подстроку вставки. Вторая перегрузка.
		/// </summary>
		/// <returns>Подстрока вставки.</returns>
		private static string InsertIdentNr(int level, bool indented, int indentSize)
		{
			return string.Format("\r\n{0}", Indent(level, indented, indentSize));
		}

		/// <summary>
		/// Подсветить специальные зарезервированные слова JavaScript.
		/// </summary>
		/// <param name="section">Исходная строка.</param>
		/// <returns>Преобразованная строка.</returns>
		private static string HighlightKeywords(string section)
		{
			List<string> reservedWords = new List<string> { "abstract", "boolean", "break", "byte", "case", "catch", "char", "class", "const", "continue", "debugger", "default", "delete", "do", "double", "else", "enum", "export", "extends", "false", "final", "finally", "float", "for", "function", "goto", "if", "implements", "import", "in", "instanceof", "int", "interface", "long", "native", "new", "null", "package", "private", "protected", "public", "return", "short", "static", "super", "switch", "synchronized", "this", "throw", "throws", "transient", "true", "try", "typeof", "var", "void", "volatile", "while", "with" };
			return reservedWords.Aggregate(section, (current, reservedWord) => Regex.Replace(current, string.Format("\\b{0}\\b", reservedWord), string.Format("<span class=\"reserved\">{0}</span>", reservedWord)));
		}
	}
}