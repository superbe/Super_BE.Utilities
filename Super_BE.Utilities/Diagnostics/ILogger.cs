using System;
using System.Web;

namespace Super_BE.Utilities.Diagnostics
{
	/// <summary>
	/// Интерфейс журнала.
	/// </summary>
	public interface ILogger
	{
		/// <summary>
		/// Залогировать сообщение. Первая перегрузка.
		/// </summary>
		/// <param name="message">Сообщение.</param>
		void Information(string message);

		/// <summary>
		/// Залогировать сообщение. Вторая перегрузка.
		/// </summary>
		/// <param name="format">Формат сообщения.</param>
		/// <param name="variables">Параметры сообщения.</param>
		void Information(string format, params object[] variables);

		/// <summary>
		/// Залогировать важное сообщение. Первая перегрузка.
		/// </summary>
		/// <param name="message">Важное сообщение.</param>
		void Warning(string message);

		/// <summary>
		/// Залогировать важное сообщение. Вторая перегрузка.
		/// </summary>
		/// <param name="format">Формат важного сообщения.</param>
		/// <param name="variables">Параметры сообщения.</param>
		void Warning(string format, params object[] variables);

		/// <summary>
		/// Трассировка задержек вызова внешней службы. Первая перегрузка.
		/// </summary>
		/// <param name="componentName">Наименование компонента.</param>
		/// <param name="method">Наименование метода.</param>
		/// <param name="timespan">Время задержки.</param>
		void TraceApi(string componentName, string method, TimeSpan timespan);

		/// <summary>
		/// Трассировка задержек вызова внешней службы. Вторая перегрузка.
		/// </summary>
		/// <param name="componentName">Наименование компонента.</param>
		/// <param name="method">Наименование метода.</param>
		/// <param name="timespan">Время задержки.</param>
		/// <param name="properties">Сообщение.</param>
		void TraceApi(string componentName, string method, TimeSpan timespan, string properties);

		/// <summary>
		/// Трассировка задержек вызова внешней службы. Третья перегрузка.
		/// </summary>
		/// <param name="componentName">Наименование компонента.</param>
		/// <param name="method">Наименование метода.</param>
		/// <param name="timespan">Время задержки.</param>
		/// <param name="format">Формат сообщения.</param>
		/// <param name="variables">Параметры сообщения.</param>
		void TraceApi(string componentName, string method, TimeSpan timespan, string format, params object[] variables);

		/// <summary>
		/// Залогировать HTTP-запрос. Первая перегрузка.
		/// </summary>
		/// <param name="request">HTTP-запрос.</param>
		void Http(HttpRequest request);

		/// <summary>
		/// Залогировать HTTP-запрос. Вторая перегрузка.
		/// </summary>
		/// <param name="request">HTTP-запрос.</param>
		/// <param name="i">Порядковый номер итерации.</param>
		void Http(HttpRequest request, int i);

		/// <summary>
		/// Залогировать ошибку. Первая перегрузка.
		/// </summary>
		/// <param name="message">Сообщение.</param>
		void Error(string message);

		/// <summary>
		/// Залогировать ошибку. Вторая перегрузка.
		/// </summary>
		/// <param name="format">Формат сообщения.</param>
		/// <param name="variables">Параметры сообщения.</param>
		void Error(string format, params object[] variables);

		/// <summary>
		/// Залогировать ошибку. Третья перегрузка.
		/// </summary>
		/// <param name="exception">Вложенное исключение.</param>
		/// <param name="format">Формат сообщения.</param>
		/// <param name="variables">Параметры сообщения.</param>
		void Error(Exception exception, string format, params object[] variables);

		/// <summary>
		/// Залогировать ошибку. Четвертая перегрузка.
		/// </summary>
		/// <param name="code">Код ошибки.</param>
		/// <param name="exception">Вложенное исключение.</param>
		/// <param name="format">Формат сообщения.</param>
		/// <param name="variables">Параметры сообщения.</param>
		void Error(int code, Exception exception, string format, params object[] variables);

		/// <summary>
		/// Залогировать ошибку. Пятая перегрузка.
		/// </summary>
		/// <param name="code">Код ошибки.</param>
		/// <param name="exception">Вложенное исключение.</param>
		/// <param name="request">HTTP-запрос.</param>
		/// <param name="format">Формат сообщения.</param>
		/// <param name="variables">Параметры сообщения.</param>
		void Error(int code, Exception exception, HttpRequest request, string format, params object[] variables);
	}
}
