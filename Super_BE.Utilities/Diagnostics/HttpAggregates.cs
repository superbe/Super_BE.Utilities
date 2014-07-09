using Super_BE.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace Super_BE.Utilities.Diagnostics
{
	[Serializable]
	[XmlRoot("PurchaseOrder", Namespace = "http://www.prognoz.ru", IsNullable = false)]
	public class HttpAggregates : List<HttpAggregateItem>
	{
		public int Add(HttpDataItem dataItem)
		{
			DateTime currentDate = dataItem.Date.Date.AddHours(dataItem.Date.Hour);
			HttpAggregateItem current = this.FirstOrDefault(x => x.Date.Date == dataItem.Date && x.Date.Hour == dataItem.Date.Hour);
			if (current.IsNull())
			{
				current = new HttpAggregateItem { Date = currentDate };
				Add(current);
			}
			return IncrementData(dataItem, current);
		}

		public void AddRange(HttpDataItem[] dataItem)
		{
			HttpDataItem[] result = dataItem.OrderBy(x => x.Date).ToArray();
			HttpAggregateItem current = null;
			foreach (HttpDataItem item in result)
			{
				DateTime currentDate = item.Date.Date.AddHours(item.Date.Hour);

				if (current.IsNull())
				{
					current = this.FirstOrDefault(x => x.Date == currentDate);
					if (current.IsNull())
					{
						current = new HttpAggregateItem { Date = currentDate };
						Add(current);
					}
				}
				if (current != null && (current.Date != currentDate))
				{
					current = this.FirstOrDefault(x => x.Date == currentDate);
					if (current.IsNull())
					{
						current = new HttpAggregateItem { Date = currentDate };
						Add(current);
					}
				}
				IncrementData(item, current);
			}
		}

		public HttpAggregates GetPerDay()
		{
			var result = new HttpAggregates();
			foreach (HttpAggregateItem item in this)
			{
				HttpAggregateItem currentItem = result.FirstOrDefault(x => x.Date == item.Date.Date);
				if (currentItem.IsNull())
				{
					currentItem = new HttpAggregateItem { Date = item.Date.Date };
					result.Add(currentItem);
				}
				IncrementData(currentItem, item);
			}
			return result;
		}

		public HttpAggregates GetPerMonth()
		{
			var result = new HttpAggregates();
			foreach (HttpAggregateItem item in this)
			{
				HttpAggregateItem currentItem = result.FirstOrDefault(x => x.Date.Year == item.Date.Year && x.Date.Month == item.Date.Month);
				if (currentItem.IsNull())
				{
					currentItem = new HttpAggregateItem { Date = new DateTime(item.Date.Year, item.Date.Month, 1) };
					result.Add(currentItem);
				}
				IncrementData(currentItem, item);
			}
			return result;
		}

		private static int IncrementData(HttpDataItem dataItem, HttpAggregateItem current)
		{
			try
			{
				if (current.IsNull() || dataItem.IsNull()) return 0;
				IncrementData(current.User, dataItem.User);
				IncrementData(current.Url, dataItem.Url != null ? dataItem.Url.OriginalString : string.Empty);
				IncrementData(current.Pages, dataItem.Url != null ? dataItem.Url.OriginalString.Split('?')[0] : string.Empty);
				IncrementData(current.Referer, dataItem.Referer != null ? dataItem.Referer.AbsolutePath : string.Empty);
				IncrementData(current.HttpMethod, dataItem.HttpMethod);
				IncrementData(current.Host, dataItem.Host);
				IncrementData(current.Ip, dataItem.Ip);
				if (!dataItem.Languages.IsNull())
				{
					foreach (string language in dataItem.Languages) IncrementData(current.Languages, language);
				}
				IncrementData(current.Browser, string.Format("{0} {1}", dataItem.BrowserName, dataItem.BrowserMajorVersion));
				IncrementData(current.Platform, dataItem.BrowserPlatform);
				IncrementData(current.Capacity, dataItem.BrowserCapacity.ToString(CultureInfo.InvariantCulture));
				if (dataItem.IsMobile) current.IsMobile++;
				current.Count++;
				return current.Count;
			}
			catch (Exception)
			{
				return 0;
			}
		}

		private static void IncrementData(Dictionary<string, int> collection, string key, int count = 1)
		{
			if (!string.IsNullOrWhiteSpace(key) && !collection.IsNull())
			{
				if (collection.ContainsKey(key))
				{
					collection[key] += count;
				}
				else
				{
					collection.Add(key, 1);
				}
			}
		}

		public void Pack()
		{
			var result = new List<HttpAggregateItem>();
			foreach (HttpAggregateItem item in this)
			{
				HttpAggregateItem currentItem = result.FirstOrDefault(x => x.Date == item.Date);
				if (currentItem == null)
					result.Add(item);
				else
					IncrementData(currentItem, item);
			}
			if (result.Count == 0) return;
			Clear();
			AddRange(result.ToArray());
		}

		private void IncrementData(HttpAggregateItem currentItem, HttpAggregateItem item)
		{
			IncrementData(currentItem.User, item.User);
			IncrementData(currentItem.Url, item.Url);
			IncrementData(currentItem.Pages, item.Pages);
			IncrementData(currentItem.Referer, item.Referer);
			IncrementData(currentItem.HttpMethod, item.HttpMethod);
			IncrementData(currentItem.Host, item.Host);
			IncrementData(currentItem.Ip, item.Ip);
			IncrementData(currentItem.Languages, item.Languages);
			IncrementData(currentItem.Browser, item.Browser);
			IncrementData(currentItem.Platform, item.Platform);
			IncrementData(currentItem.Capacity, item.Capacity);
			currentItem.IsMobile += item.IsMobile;
			currentItem.Count += item.Count;
		}

		private void IncrementData(Dictionary<string, int> dataItem, Dictionary<string, int> current)
		{
			foreach (KeyValuePair<string, int> pair in current)
			{
				IncrementData(dataItem, pair.Key, pair.Value);
			}
		}
	}
}