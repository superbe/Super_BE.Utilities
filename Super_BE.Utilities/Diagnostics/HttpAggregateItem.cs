using System;
using System.Collections.Generic;
using System.Linq;

namespace Super_BE.Utilities.Diagnostics
{
	[Serializable]
	public class HttpAggregateItem
	{
		/// <summary>
		/// Дата запроса.
		/// </summary>
		public DateTime Date { get; set; }

		/// <summary>
		/// Пользователи.
		/// </summary>
		public Dictionary<string, int> User { get; set; }

		/// <summary>
		/// Урла текущая.
		/// </summary>
		public Dictionary<string, int> Url { get; set; }

		/// <summary>
		/// Запрошенная страница.
		/// </summary>
		public Dictionary<string, int> Pages { get; set; }

		/// <summary>
		/// Урла отсылки.
		/// </summary>
		public Dictionary<string, int> Referer { get; set; }

		/// <summary>
		/// Метод запроса.
		/// </summary>
		public Dictionary<string, int> HttpMethod { get; set; }

		/// <summary>
		/// Имя хоста.
		/// </summary>
		public Dictionary<string, int> Host { get; set; }

		/// <summary>
		/// Ip-адрес.
		/// </summary>
		public Dictionary<string, int> Ip { get; set; }

		/// <summary>
		/// Мобильность.
		/// </summary>
		public int IsMobile { get; set; }

		/// <summary>
		/// Языки.
		/// </summary>
		public Dictionary<string, int> Languages { get; set; }

		/// <summary>
		/// Браузеры.
		/// </summary>
		public Dictionary<string, int> Browser { get; set; }

		/// <summary>
		/// Операционная система.
		/// </summary>
		public Dictionary<string, int> Platform { get; set; }

		/// <summary>
		/// Разрядность браузера (16, 32).
		/// </summary>
		public Dictionary<string, int> Capacity { get; set; }

		/// <summary>
		/// Количество запросов за период.
		/// </summary>
		public int Count { get; set; }

		public HttpAggregateItem()
		{
			User = new Dictionary<string, int>();
			Url = new Dictionary<string, int>();
			Pages = new Dictionary<string, int>();
			Referer = new Dictionary<string, int>();
			HttpMethod = new Dictionary<string, int>();
			Host = new Dictionary<string, int>();
			Ip = new Dictionary<string, int>();
			Languages = new Dictionary<string, int>();
			Browser = new Dictionary<string, int>();
			Platform = new Dictionary<string, int>();
			Capacity = new Dictionary<string, int>();
		}

		public int CountHosts
		{
			get
			{
				return Host.Sum(pair => pair.Value);
			}
		}

		public int CountIp
		{
			get
			{
				return Ip.Sum(pair => pair.Value);
			}
		}

		public int CountUsers
		{
			get
			{
				return User.Sum(pair => pair.Value);
			}
		}
	}
}