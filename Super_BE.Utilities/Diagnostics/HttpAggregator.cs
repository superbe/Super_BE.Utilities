using Super_BE.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net.Mime;
using System.Web;

namespace Super_BE.Utilities.Diagnostics
{
	public class HttpAggregator
	{
		// Поля.
		private const string SearchErrorsPattern = "*_http.log";
		private const string AggregatesFileName = "aggregates.zip";
		private const string DocumentName = "data.bin";
		private readonly Uri _documentPath = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), new Uri(DocumentName, UriKind.Relative));

		/// <summary>
		/// Закэшированный в приложение агрегат.
		/// </summary>
		protected HttpAggregates Aggregates
		{
			get { return (HttpAggregates)HttpContext.Current.Application["Tip_NNovSystem_Diagnostics_HttpAggregator_Aggregates"]; }
			set { HttpContext.Current.Application["Tip_NNovSystem_Diagnostics_HttpAggregator_Aggregates"] = value; }
		}

		/// <summary>
		/// Агрегировать данные запросов. Первая перегрузка.
		/// </summary>
		/// <param name="path">Относительный путь к папке хранения.</param>
		public void Aggregate(string path = "~/Logs/Http/")
		{
			string folderName = HttpContext.Current.Server.MapPath(path);
			if (GetFilesList(folderName).Length == 0) return;
			using (Package package = Package.Open(Path.Combine(folderName, AggregatesFileName), FileMode.OpenOrCreate))
			{
				// Создали агрегатор.
				var result = new HttpAggregates();
				// Загрузили данные агрегации из хранилища.
				if (package.PartExists(_documentPath)) result = LoadDocument(package);
				// Добавили новые данные из логов.
				AggregateFiles(result, folderName);
				// Упаковали (избавились от дубликатов).
				result.Pack();
				// Сохранили в хранилище.
				SaveDocument(package, result);
			}
		}

		/// <summary>
		/// Агрегировать данные запросов. Вторая перегрузка.
		/// </summary>
		/// <param name="package">Пакет архивирования.</param>
		/// <param name="folderName">Относительный путь к папке хранения.</param>
		/// <returns></returns>
		public HttpAggregates Aggregate(Package package, string folderName)
		{
			// Создали агрегатор.
			var result = new HttpAggregates();
			// Загрузили данные агрегации из хранилища.
			if (package.PartExists(_documentPath)) result = LoadDocument(package);
			// Добавили новые данные из логов.
			AggregateFiles(result, folderName);
			// Упаковали (избавились от дубликатов).
			result.Pack();
			// Сохранили в хранилище.
			SaveDocument(package, result);
			// Вернули агрегат.
			return result;
		}

		public Attendance[] GetAttendance(DateTime date, HttpPeriod period, string path = "~/Logs/Http/")
		{
			switch (period)
			{
				case HttpPeriod.Day:
					return GetAttendance(date, 1, path);
				case HttpPeriod.Week:
					return GetAttendance(date, 7, path);
				case HttpPeriod.Month:
					return GetAttendance(date, DateTime.DaysInMonth(date.Year, date.Month), path);
				case HttpPeriod.Year:
					return GetAttendance(date, DateTime.IsLeapYear(date.Year) ? 366 : 365, path);
				default:
					return GetAttendance(date, -1, path);
			}
		}

		/// <summary>
		/// Посещаемость сайта.
		/// </summary>
		/// <param name="date">Дата на которую берется выборка.</param>
		/// <param name="period">Период в днях.</param>
		/// <param name="path">Относительный путь к папке хранения.</param>
		/// <returns>Выборка.</returns>
		private Attendance[] GetAttendance(DateTime date, int period, string path = "~/Logs/Http/")
		{
			// Подготовили переменные.
			if (string.IsNullOrWhiteSpace(path)) path = "~/Logs/Http/";
			string folderName = HttpContext.Current.Server.MapPath(path);
			DateTime today = date != DateTime.MinValue && date != DateTime.MaxValue ? date.Date.AddDays(1) : DateTime.Now.Date.AddDays(1);
			DateTime yesterday = today.AddDays(-period);
			// Открыли пакет.
			using (Package package = Package.Open(Path.Combine(folderName, AggregatesFileName), FileMode.Open))
			{
				// Получили выборку.
				IEnumerable<HttpAggregateItem> aggregates = period == 1 ? Init(package, folderName) : (period > 1 && period <= 31 ? Init(package, folderName).GetPerDay() : Init(package, folderName).GetPerMonth());
				var result = InitializationResult(period);
				DateTime weekStartDay = today.AddDays(1 - Convert.ToInt32(today.DayOfWeek));
				for (int i = 0; i < result.Length; i++)
				{
					if (period == 1)
						result[i] = new Attendance(today.Date.AddDays(-1).AddHours(i), today.Date.AddHours(i).ToString("t"));
					if (period == 7)
					{
						DateTime currentDate = weekStartDay.AddDays(i);
						result[i] = new Attendance(currentDate, string.Format(CultureInfo.CurrentCulture, "{0:ddd} ({0:d})", currentDate));
					}
					if (period > 7 && period < 32)
					{
						DateTime currentDate = today.Date.Date.AddDays(1 - today.Day).AddDays(i);
						result[i] = new Attendance(currentDate, currentDate.ToString("d"));
					}
					if (period >= 365)
					{
						DateTime currentDate = new DateTime(today.Year, 1, 1).AddMonths(i);
						result[i] = new Attendance(currentDate, currentDate.ToString("y", CultureInfo.CurrentCulture));
					}
				}
				//Array.Reverse(result);

				foreach (HttpAggregateItem aggregate in aggregates)
				{
					if (period == -1 && aggregate.Date > DateTime.MinValue && aggregate.Date < DateTime.MaxValue)
					{
						Array.Resize(ref result, result.Length + 1);
						result[result.Length - 1] = new Attendance(aggregate, GetMarkerName(aggregate, period));
					}
					else
						if (aggregate.Date < today && aggregate.Date > yesterday)
						{
							if (period == 1)
								result[aggregate.Date.Hour] = new Attendance(aggregate, GetMarkerName(aggregate, period));
							if (period == 7 && aggregate.Date >= weekStartDay)
								result[Convert.ToInt32(aggregate.Date.DayOfWeek) - 1] = new Attendance(aggregate, GetMarkerName(aggregate, period));
							if (period > 7 && period < 32 && aggregate.Date.Day - 1 < result.Length)
								result[aggregate.Date.Day - 1] = new Attendance(aggregate, GetMarkerName(aggregate, period));
							if (period >= 365 && aggregate.Date > new DateTime(aggregate.Date.Year, 1, 1))
							{
								//Array.Resize(ref result, result.Length + 1);
								//result[result.Length - 1] = new Attendance(aggregate, GetMarkerName(aggregate, period));
								result[aggregate.Date.Month - 1] = new Attendance(aggregate, GetMarkerName(aggregate, period));
							}
						}
				}
				// Вернули выборку.
				//Array.Reverse(result);
				return result;
			}
		}

		private static Attendance[] InitializationResult(int period)
		{
			if (period == 1) return new Attendance[24];
			if (period == 7) return new Attendance[7];
			if (period > 7 && period < 32) return new Attendance[period];
			if (period >= 365) return new Attendance[12];
			return new Attendance[0];
		}

		private string GetMarkerName(HttpAggregateItem aggregate, int period)
		{
			if (period == 1)
				return aggregate.Date.ToString("t");
			if (period == 7)
				return string.Format("{0:ddd} ({0:d})", aggregate.Date);
			if (period == -1 || period >= 365)
				return aggregate.Date.ToString("y");
			return aggregate.Date.ToString("d");
		}

		private HttpAggregates Init(Package package, string folderName)
		{
			if (Aggregates == null || Aggregates.Count(x => x.Date.Date == DateTime.Now.Date && x.Date.Hour == DateTime.Now.Hour) == 0)
				Aggregates = Aggregate(package, folderName);
			return Aggregates;
		}


		private void SaveDocument(Package package, HttpAggregates result)
		{
			// Сформировали путь к документу.
			PackagePart document = package.PartExists(_documentPath) ? package.GetPart(_documentPath) : package.CreatePart(_documentPath, MediaTypeNames.Text.Plain, CompressionOption.Normal);
			using (Stream stream = new MemoryStream(result.ToBinary()))
			{
				try
				{
					if (document != null) CopyStream(stream, document.GetStream());
				}
				finally
				{
					stream.Close();
				}
			}
		}

		private static void CopyStream(Stream source, Stream target)
		{
			const int bufSize = 0x1000;
			var buf = new byte[bufSize];
			int bytesRead;
			while ((bytesRead = source.Read(buf, 0, bufSize)) > 0) target.Write(buf, 0, bytesRead);
		}

		private HttpAggregates LoadDocument(Package package)
		{
			// Получили документ.
			PackagePart document = package.GetPart(_documentPath);
			// Прочитали документ из потока.
			using (var reader = document.GetStream())
			{
				try
				{
					var buffer = new byte[reader.Length];
					reader.Read(buffer, 0, buffer.Length);
					return buffer.FromBinary<HttpAggregates>();
				}
				finally
				{
					reader.Close();
				}
			}
		}


		/// <summary>
		/// Агрегировать все файлы.
		/// </summary>
		/// <param name="result"> </param>
		/// <param name="folderName"></param>
		/// <returns></returns>
		private void AggregateFiles(HttpAggregates result, string folderName)
		{
			if (result == null) return;
			if (Directory.Exists(folderName))
			{
				FileInfo[] fileInfo = GetFilesList(folderName);
				var dataItems = new HttpDataItem[fileInfo.Length];
				for (int i = 0; i < fileInfo.Length; i++)
				{
					if (fileInfo[i].Exists)
					{
						if (result.Add((HttpDataItem)dataItems[i].Load(fileInfo[i].FullName, TypeConvert.Binary)) > 0)
						{
							fileInfo[i].Delete();
						}
					}
				}
			}
		}

		/// <summary>
		/// Получить список файлов, удовлетворяющих условиям фильтрации.
		/// </summary>
		/// <param name="path">Путь к папке харнения.</param>
		/// <returns>Список файлов, удовлетворяющих условиям фильтрации.</returns>
		private static FileInfo[] GetFilesList(string path = "~/Logs/Http/")
		{
			return (new DirectoryInfo(path)).GetFiles(SearchErrorsPattern, SearchOption.TopDirectoryOnly).ToArray();
		}
	}
}