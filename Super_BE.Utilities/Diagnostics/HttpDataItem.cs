using System;
using System.Security.Permissions;

namespace Super_BE.Utilities.Diagnostics
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	public class HttpDataItem
	{
		/// <summary>
		/// Дата запроса.
		/// </summary>
		public DateTime Date { get; set; }

		/// <summary>
		/// Пользователь.
		/// </summary>
		public string User { get; set; }

		/// <summary>
		/// URL-адрес запроса.
		/// </summary>
		public Uri Url { get; set; }

		/// <summary>
		/// URL-адрес с которого был осуществлен переход.
		/// </summary>
		public Uri Referer { get; set; }

		/// <summary>
		/// Метод запроса (GET, POST...)
		/// </summary>
		public string HttpMethod { get; set; }

		/// <summary>
		/// Хост клиента.
		/// </summary>
		public string Host { get; set; }

		/// <summary>
		/// IP-адрес клиента.
		/// </summary>
		public string Ip { get; set; }

		/// <summary>
		/// Агент клиента.
		/// </summary>
		public string Agent { get; set; }

		/// <summary>
		/// Мобильность агента.
		/// </summary>
		public bool IsMobile { get; set; }

		/// <summary>
		/// Язык агента клиента.
		/// </summary>
		public string[] Languages { get; set; }
	}
}