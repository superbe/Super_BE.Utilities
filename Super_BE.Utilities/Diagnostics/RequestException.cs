using Super_BE.Utilities.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_BE.Utilities.Diagnostics
{
	/// <summary>
	/// Исключение запроса сайта.
	/// </summary>
	class RequestException : Exception
	{
		/// <summary>
		/// Код ошибки.
		/// </summary>
		/// <remarks>Задается.</remarks>
		public int Code { get; set; }

		/// <summary>
		/// Url странички с ошибкой.
		/// </summary>
		/// <remarks>Автоматически.</remarks>
		public HttpDataItem Request { get; set; }

		/// <summary>
		/// Дата возникновения ошибки.
		/// </summary>
		/// <remarks>Автоматически.</remarks>
		public DateTime Date { get; set; }

		/// <summary>
		/// Ползователь под которым произошла ошибка.
		/// </summary>
		/// <remarks>Автоматически.</remarks>
		public string User { get; set; }

		/// <summary>
		/// Конструктор. Первая перегрузка.
		/// </summary>
		public RequestException()
		{

		}

		/// <summary>
		/// Конструктор. Вторая перегрузка.
		/// </summary>
		/// <param name="message">Сообщение.</param>
		/// <param name="code">Код ошибки.</param>
		/// <param name="request">Http-запрос, приведший к ошибке.</param>
		/// <param name="date">Время возникновения ошибки.</param>
		/// <param name="user">Авторизованный пользователь, действия которого привели к ошибке.</param>
		public RequestException(string message, int? code = null, HttpDataItem request = null, DateTime? date = null, string user = null)
			: base(message)
		{
			if (code != null) Code = (int)code;
			if (request != null) Request = request;
			if (date != null) Date = (DateTime)date;
			if (!string.IsNullOrWhiteSpace(user)) User = user;
		}

		/// <summary>
		/// Конструктор. Третья перегрузка.
		/// </summary>
		/// <param name="message">Сообщение.</param>
		/// <param name="code">Код ошибки.</param>
		/// <param name="request">Http-запрос, приведший к ошибке.</param>
		/// <param name="date">Время возникновения ошибки.</param>
		/// <param name="user">Авторизованный пользователь, действия которого привели к ошибке.</param>
		/// <param name="exception">Вложенное исключение.</param>
		public RequestException(string message, Exception exception, int? code = null, HttpDataItem request = null, DateTime? date = null, string user = null)
			: base(message, exception)
		{
			if (code != null) Code = (int)code;
			if (request != null) Request = request;
			if (date != null) Date = (DateTime)date;
			if (!string.IsNullOrWhiteSpace(user)) User = user;
		}
	}
}
