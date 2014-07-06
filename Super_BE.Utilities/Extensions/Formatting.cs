using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Super_BE.Utilities.Extensions
{
	/// <summary>
	/// Класс форматирования Json-строки для отображения на html-страничке.
	/// </summary>
	/// TODO: сделать как расширение.
	public class Formatting
	{
		/// <summary>
		/// Флаг форматирования массива.
		/// </summary>
		private bool _isArray;

		/// <summary>
		/// Флаг форматирования строки.
		/// </summary>
		private bool _isString;

		/// <summary>
		/// Знак начала строки.
		/// </summary>
		private char _newlineCharacter;

		/// <summary>
		/// Уровень отступа.
		/// </summary>
		private int _level;

		/// <summary>
		/// Добавлять отступы.
		/// </summary>
		private readonly bool _indented;

		/// <summary>
		/// Кодировать спецсимволы для html-представления.
		/// </summary>
		private readonly bool _toEncode;

		/// <summary>
		/// Выделять ключевые слова JavaScript.
		/// </summary>
		private readonly bool _toHighlight;

		private readonly bool _transferArrays;
		private readonly bool _transferObjects;

		/// <summary>
		/// Специальные символы форматирования.
		/// </summary>
		private readonly char[] _specialСharacters = { '{', '}', ',', '[', ']', '(', ')', '\\', '\'', '"' };

		/// <summary>
		/// Величина отступа.
		/// </summary>
		private readonly int _indentSize;

		/// <summary>
		/// Ключевые слова JavaScript.
		/// </summary>
		private readonly List<string> _reservedWords = new List<string> { "abstract", "boolean", "break", "byte", "case", "catch", "char", "class", "const", "continue", "debugger", "default", "delete", "do", "double", "else", "enum", "export", "extends", "false", "final", "finally", "float", "for", "function", "goto", "if", "implements", "import", "in", "instanceof", "int", "interface", "long", "native", "new", "null", "package", "private", "protected", "public", "return", "short", "static", "super", "switch", "synchronized", "this", "throw", "throws", "transient", "true", "try", "typeof", "var", "void", "volatile", "while", "with" };

		/// <summary>
		/// Конструктор.
		/// </summary>
		/// <param name="indentSize">Величина отступа.</param>
		/// <param name="indented">Добавлять отступы.</param>
		/// <param name="toEncode">Кодировать спецсимволы для html-представления.</param>
		/// <param name="toHighlight">Выделять ключевые слова JavaScript.</param>
		/// <param name="transferArrays">Переносить в массивах.</param>
		/// <param name="transferObjects">Переносить объекты.</param>
		public Formatting(int indentSize = 4, bool indented = true, bool toEncode = true, bool toHighlight = true, bool transferArrays = false, bool transferObjects = false)
		{
			_toEncode = toEncode;
			_toHighlight = toHighlight;
			_transferArrays = transferArrays;
			_transferObjects = transferObjects;
			_indentSize = indentSize;
			_indented = indented;
		}

		/// <summary>
		/// Выполнить форматирование строки.
		/// </summary>
		/// <param name="source">Строка кода, подлежащая форматированию.</param>
		/// <returns>Отформатированнная строка кода.</returns>
		public string Run(string source)
		{
			//source = source.Replace("\\\\\",", "\",").Replace("\\\\", "\\").Replace("\\\\", "\\").Replace("\\\\", "\\");
			//source = source.Replace("\",\"", "\", \"").Replace("\\\\", "\/");
			if (string.IsNullOrEmpty(source)) return string.Empty;
			string result = null;
			for (int index = 0; index < source.Length; index++)
			{
				char c = source[index];
				if (_specialСharacters.Any(x => x == c))
				{
					if (_isString)
					{
						if (index >= 0 && (_newlineCharacter == c && source[index - 1] != '\\')) _isString = false;
						//if (index >= 0 && ((_newlineCharacter == c && source[index - 1] != '\\') || (_newlineCharacter == c && source[index - 1] == '\\' && source[index - 2] == '\\'))) _isString = false;
						result += c;
					}
					else
					{
						if (c == '\'' || c == '"')
						{
							_isString = true;
							_newlineCharacter = c;
							result += c;
						}
						else
						{
							string subString;
							switch (c)
							{
								case '{':
									result += c;
									_level++;
									subString = (source[index + 1] == '}' || (source[index + 1] == ' ' && source[index + 2] == '}') || (source[index + 1] == ' ' && source[index + 2] == ' ' && source[index + 3] == '}')) ? string.Empty : InsertIdentNr();
									if (!string.IsNullOrEmpty(InsertIdentNr()) && index < source.Length && source[index + 1] == ' ') index++;
									result += subString;
									break;
								case '}':
									_level--;
									result += (source[index - 1] == '{' || (source[index - 1] == ' ' && source[index - 2] == '{') || (source[index - 1] == ' ' && source[index - 2] == ' ' && source[index - 3] == '{')) ? string.Empty : InsertIdentNr();
									result += c;
									break;
								case '[':
									result += c;
									_isArray = true;
									if (_transferArrays)
									{
										_level++;
										subString = (source[index + 1] == ']' || (source[index + 1] == ' ' && source[index + 2] == ']') || (source[index + 1] == ' ' && source[index + 2] == ' ' && source[index + 3] == ']')) ? string.Empty : InsertIdentNr();
										if (!string.IsNullOrEmpty(InsertIdentNr()) && index < source.Length && source[index + 1] == ' ') index++;
										result += subString;
									}
									break;
								case ']':
									if (_transferArrays)
									{
										_level--;
										result += (source[index - 1] == '[' || (source[index - 1] == ' ' && source[index - 2] == '[') || (source[index - 1] == ' ' && source[index - 2] == ' ' && source[index - 3] == '[')) ? string.Empty : InsertIdentNr();
									}
									result += c;
									_isArray = false;
									break;
								case ',':
									result += c;
									subString = InsertIdentNr(source, index);
									if (!string.IsNullOrEmpty(subString) && index < source.Length && source[index + 1] == ' ') index++;
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
			return _toHighlight ? HighlightKeywords(_toEncode ? HttpUtility.HtmlEncode(result) : result) : _toEncode ? HttpUtility.HtmlEncode(result) : result;
		}

		/// <summary>
		/// Сформировать отступ.
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		public string Indent(int level)
		{
			if (!_indented) return string.Empty;
			string result = null;
			int length = level * _indentSize;
			for (int i = 0; i < length; i++) result += " ";
			return result;
		}

		/// <summary>
		/// Сформировать подстроку вставки. Первая перегрузка.
		/// </summary>
		/// <param name="source">Исходная строка форматирования.</param>
		/// <param name="index">Индекс специального знака в строке.</param>
		/// <returns>Подстрока вставки.</returns>
		private string InsertIdentNr(string source, int index)
		{
			if (!_transferArrays && _isArray) return string.Empty;
			if (source[index - 1] == '"' && source[index - 2] == '\\' && source[index - 3] == '\\') return InsertIdentNr();
			if (!_transferObjects && index >= 0 && source[index - 1] == '}' && source[index + 1] == ' ' && source[index + 2] == '{') return string.Empty;
			return InsertIdentNr();
		}

		/// <summary>
		/// Сформировать подстроку вставки. Вторая перегрузка.
		/// </summary>
		/// <returns>Подстрока вставки.</returns>
		private string InsertIdentNr()
		{
			return string.Format("\r\n{0}", Indent(_level));
		}

		/// <summary>
		/// Подсветить специальные зарезервированные слова JavaScript.
		/// </summary>
		/// <param name="section">Исходная строка.</param>
		/// <returns>Преобразованная строка.</returns>
		private string HighlightKeywords(string section)
		{
			return _reservedWords.Aggregate(section, (current, reservedWord) => Regex.Replace(current, string.Format("\\b{0}\\b", reservedWord), string.Format("<span class=\"reserved\">{0}</span>", reservedWord)));
		}
	}
}