using Super_BE.Utilities.Extensions;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;

namespace Super_BE.Utilities.Diagnostics
{
	/// <summary>
	/// Класс журналирования.
	/// </summary>
	public class Logger : ILogger
	{
		string _path;
		string _user;

		#region Конструктор.
		/// <summary>
		/// Конструктор. Первая перегрузка.
		/// </summary>
		/// <param name="path">Путь к папке журналирования.</param>
		public Logger(string path = "")
		{
			if (!string.IsNullOrWhiteSpace(path))
				_path = path;
		}

		/// <summary>
		/// Конструктор. Вторая перегрузка.
		/// </summary>
		/// <param name="path">Путь к папке журналирования.</param>
		/// <param name="user">Учетная запись пользователя.</param>
		public Logger(string user, string path = "")
		{
			if (!string.IsNullOrWhiteSpace(path))
				_path = path;
			_user = user;
		}
		#endregion Конструктор.

		#region Реализация.
		/// <summary>
		/// Реализация объекта. Первая перегрузка.
		/// </summary>
		/// <returns>Объект журналирования.</returns>
		public static Logger Implementation()
		{
			return new Logger();
		}

		/// <summary>
		/// Реализация объекта. Вторая перегрузка.
		/// </summary>
		/// <param name="path">Путь к папке журналирования.</param>
		/// <returns>Объект журналирования.</returns>
		public static Logger Implementation(string path = "")
		{
			return new Logger(path);
		}

		/// <summary>
		/// Реализация объекта. Третья перегрузка.
		/// </summary>
		/// <param name="path">Путь к папке журналирования.</param>
		/// <param name="user">Учетная запись пользователя.</param>
		/// <returns>Объект журналирования.</returns>
		public static Logger Implementation(string user, string path = "")
		{
			return new Logger(user, path);
		}
		#endregion Реализация.

		#region Information.
		/// <summary>
		/// Журналировать сообщение. Первая перегрузка.
		/// </summary>
		/// <param name="message">Сообщение.</param>
		public void Information(string message)
		{
#if DEBUG
			Trace.TraceInformation(message);
#endif
			SaveMessage(message, "Logs/Informations/", "{0}_informations");
		}

		/// <summary>
		/// Журналировать сообщение. Вторая перегрузка.
		/// </summary>
		/// <param name="format">Формат сообщения.</param>
		/// <param name="variables">Параметры сообщения.</param>
		public void Information(string format, params object[] variables)
		{
			Information(string.Format(format, variables));
		}
		#endregion Information.

		#region Warning.
		/// <summary>
		/// Залогировать важное сообщение. Первая перегрузка.
		/// </summary>
		/// <param name="message">Важное сообщение.</param>
		public void Warning(string message)
		{
#if DEBUG
			Trace.TraceWarning(message);
#endif
			SaveMessage(message, "Logs/Warning/", "{0}_warning");
		}

		/// <summary>
		/// Залогировать важное сообщение. Вторая перегрузка.
		/// </summary>
		/// <param name="format">Формат важного сообщения.</param>
		/// <param name="variables">Параметры сообщения.</param>
		public void Warning(string format, params object[] variables)
		{
			Warning(string.Format(format, variables));
		}
		#endregion Warning.

		#region TraceApi.
		/// <summary>
		/// Трассировка задержек вызова внешней службы. Первая перегрузка.
		/// </summary>
		/// <param name="componentName">Наименование компонента.</param>
		/// <param name="method">Наименование метода.</param>
		/// <param name="timespan">Время задержки.</param>
		public void TraceApi(string componentName, string method, TimeSpan timespan)
		{
			TraceApi(componentName, method, timespan, "");
		}

		/// <summary>
		/// Трассировка задержек вызова внешней службы. Вторая перегрузка.
		/// </summary>
		/// <param name="componentName">Наименование компонента.</param>
		/// <param name="method">Наименование метода.</param>
		/// <param name="timespan">Время задержки.</param>
		/// <param name="properties">Сообщение.</param>
		public void TraceApi(string componentName, string method, TimeSpan timespan, string properties)
		{
			string message = String.Concat("Component:", componentName, "; Method:", method, "; Timespan:", timespan.ToString(), "; Properties:", properties);
#if DEBUG
			Trace.TraceInformation(message);
#endif
			if (Convert.ToBoolean(ConfigurationManager.AppSettings.Get("trace")))
				SaveMessage(message, "Logs/Trace/", "{0}_trace");
		}

		/// <summary>
		/// Трассировка задержек вызова внешней службы. Третья перегрузка.
		/// </summary>
		/// <param name="componentName">Наименование компонента.</param>
		/// <param name="method">Наименование метода.</param>
		/// <param name="timespan">Время задержки.</param>
		/// <param name="format">Формат сообщения.</param>
		/// <param name="variables">Параметры сообщения.</param>
		public void TraceApi(string componentName, string method, TimeSpan timespan, string format, params object[] variables)
		{
			TraceApi(componentName, method, timespan, string.Format(format, variables));
		}
		#endregion TraceApi.

		#region Http.
		/// <summary>
		/// Залогировать HTTP-запрос. Первая перегрузка.
		/// </summary>
		/// <param name="request">HTTP-запрос.</param>
		public void Http(HttpRequest request)
		{
			Http(request, 0);
		}

		/// <summary>
		/// Залогировать HTTP-запрос. Вторая перегрузка.
		/// </summary>
		/// <param name="request">HTTP-запрос.</param>
		/// <param name="i">Порядковый номер итерации.</param>
		public void Http(HttpRequest request, int i)
		{
			if (i > 10) return;
			if (string.IsNullOrWhiteSpace(_path)) _path = "Logs/Http/";
			string fileName = GetFullFileName(string.Format("{0}_http", GetFileName(_path)));
			try
			{
				(request == null ? null : new HttpDataItem
				{
					Agent = request.UserAgent,
					Date = DateTime.Now,
					Host = request.UserHostName,
					Languages = request.UserLanguages,
					HttpMethod = request.HttpMethod,
					Ip = request.UserHostAddress,
					IsMobile = request.Browser.IsMobileDevice,
					Referer = request.UrlReferrer,
					Url = request.Url,
					User = _user
				}).Save(fileName, TypeConvert.Json);
			}
			catch (Exception exception)
			{
				Error(0, exception, request, "Ошибка логирования HTTP-запроса. Итерация: {0}", i);
				Http(request, i + 1);
			}
		}
		#endregion Http.

		#region Error.
		/// <summary>
		/// Залогировать ошибку. Первая перегрузка.
		/// </summary>
		/// <param name="message">Сообщение.</param>
		public void Error(string message)
		{
			Error(message, null);
		}

		/// <summary>
		/// Залогировать ошибку. Вторая перегрузка.
		/// </summary>
		/// <param name="format">Формат сообщения.</param>
		/// <param name="variables">Параметры сообщения.</param>
		public void Error(string format, params object[] variables)
		{
			Error(null, format, variables);
		}

		/// <summary>
		/// Залогировать ошибку. Третья перегрузка.
		/// </summary>
		/// <param name="exception">Вложенное исключение.</param>
		/// <param name="format">Формат сообщения.</param>
		/// <param name="variables">Параметры сообщения.</param>
		public void Error(Exception exception, string format, params object[] variables)
		{
			Error(-1, exception, format, variables);
		}

		/// <summary>
		/// Залогировать ошибку. Четвертая перегрузка.
		/// </summary>
		/// <param name="code">Код ошибки.</param>
		/// <param name="exception">Вложенное исключение.</param>
		/// <param name="format">Формат сообщения.</param>
		/// <param name="variables">Параметры сообщения.</param>
		public void Error(int code, Exception exception, string format, params object[] variables)
		{
			Error(code, exception, null, format, variables);
		}

		/// <summary>
		/// Залогировать ошибку. Пятая перегрузка.
		/// </summary>
		/// <param name="code">Код ошибки.</param>
		/// <param name="exception">Вложенное исключение.</param>
		/// <param name="request">HTTP-запрос.</param>
		/// <param name="format">Формат сообщения.</param>
		/// <param name="variables">Параметры сообщения.</param>
		public void Error(int code, Exception exception, HttpRequest request, string format, params object[] variables)
		{
#if DEBUG
			Trace.TraceError(FormatExceptionMessage(exception, format, variables));
#endif
			if (string.IsNullOrWhiteSpace(_path)) _path = "Logs/Errors/";
			string fileName = GetFullFileName(string.Format("{0}_error", GetFileName(_path)));
			(new RequestException(string.Format(format, variables), exception, code, request == null ? null : new HttpDataItem
				{
					Agent = request.UserAgent,
					Date = DateTime.Now,
					Host = request.UserHostName,
					Languages = request.UserLanguages,
					HttpMethod = request.HttpMethod,
					Ip = request.UserHostAddress,
					IsMobile = request.Browser.IsMobileDevice,
					Referer = request.UrlReferrer,
					Url = request.Url,
					User = _user
				}, DateTime.Now, _user)).Save(fileName, TypeConvert.Json);
		}
		#endregion Error.

		#region Системные методы.
		/// <summary>
		/// Отформатировать исключение.
		/// </summary>
		/// <param name="exception">Исключение.</param>
		/// <param name="format">Формат сообщения.</param>
		/// <param name="variables">Параметры форматирования.</param>
		/// <returns>Форматированное сообщение об ошибке.</returns>
		private string FormatExceptionMessage(Exception exception, string format, params object[] variables)
		{
			var result = new StringBuilder();
			result.Append(string.Format(format, variables));
			result.Append(" Exception: ");
			result.Append(exception.ToString());
			return result.ToString();
		}

		/// <summary>
		/// Сохранить сообщение.
		/// </summary>
		/// <param name="message">Сообщение.</param>
		/// <param name="path">Путь хранения.</param>
		/// <param name="fileNameFormat">Формат хранения файла.</param>
		private void SaveMessage(string message, string path, string fileNameFormat)
		{
			if (string.IsNullOrWhiteSpace(_path)) _path = path;
			string fileName = GetFullFileName(string.Format(fileNameFormat, GetFileName(_path)));
			File.WriteAllText(fileName, message);
		}

		/// <summary>
		/// Получить полное наименование файла ошибки.
		/// </summary>
		/// <param name="fileName">Наименование файла ошибки.</param>
		/// <returns>Полное наименование файла ошибки.</returns>
		private string GetFullFileName(string fileName)
		{
			int i = 0;
			string result = string.Format("{0}.log", fileName);
			// Проверяем на существование.
			while (File.Exists(result))
			{
				result = string.Format("{0}_{1}.log", fileName, i.ToString("0"));
				i++;
			}
			return result;
		}

		/// <summary>
		/// Сформировать путь сохранения исключения.
		/// </summary>
		/// <param name="path">Начальный путь.</param>
		/// <returns>Полное наименование файла для сохранения (без расширения).</returns>
		private string GetFileName(string path)
		{
			string result = Path.Combine((new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location)).DirectoryName, path);
			if (!Directory.Exists(result)) Directory.CreateDirectory(result);
			return string.Format("{0}{1}", result, Guid.NewGuid());
		}

		/// <summary>
		/// Преобразовать массив строк в одну в Json-представлении.
		/// </summary>
		/// <param name="queryStrings">Массив исходных строк.</param>
		/// <returns>Json-представление.</returns>
		private string GetStrings(string[] queryStrings)
		{
			if (queryStrings.IsNull()) return string.Empty;
			var result = new string[queryStrings.Length];
			for (int index = 0; index < queryStrings.Length; index++)
				result[index] = string.Format("\"{0}\"", queryStrings[index]);
			return string.Format("[{0}]", string.Join(",", result));
		}
		#endregion Системные методы.
	}
}
