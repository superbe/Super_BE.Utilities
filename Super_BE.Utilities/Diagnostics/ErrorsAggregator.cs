using System;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net.Mime;
using System.Web;

namespace Super_BE.Utilities.Diagnostics
{
	public class ErrorsAggregator
	{
		private const string SearchErrorsPattern = "*_error.log";

		/// <summary>
		/// Получить список ошибок.
		/// </summary>
		/// <param name="size">Размер страницы.</param>
		/// <param name="text">Фильтрация по тексту.</param>
		/// <param name="path">Путь к папке хранения.</param>
		/// <param name="page">Номер страницы.</param>
		/// <returns>Список ошибок.</returns>
		public Errors GetErrors(int page, int size, string text, string path = "~/Logs/Errors/")
		{
			var result = new Errors();
			string folderName = HttpContext.Current.Server.MapPath(path);
			if (Directory.Exists(folderName))
			{
				FileInfo[] allFiles = GetFilesList(folderName, text);
				result.PageCount = allFiles.Length / size + ((allFiles.Length % size == 0) ? 0 : 1);
				result.Page = page;

				var i = (page - 1) * size;
				FileInfo[] files = allFiles.Skip(i).Take(size).ToArray();
				result.Items.AddRange(files.Select(fileInfo => new ErrorItem
					{
						FileName = fileInfo.Name,
						Date = fileInfo.LastWriteTimeUtc.ToString("d"),
						Context = File.ReadAllText(fileInfo.FullName)
					}));

				if (!string.IsNullOrWhiteSpace(text))
				{

				}
			}
			return result;
		}

		/// <summary>
		/// Получить список файлов, удовлетворяющих условиям фильтрации.
		/// </summary>
		/// <param name="folderName">Путь к папке харнения.</param>
		/// <param name="text">Текст по которому будет производится фильтрация.</param>
		/// <returns>Список файлов, удовлетворяющих условиям фильтрации.</returns>
		private static FileInfo[] GetFilesList(string folderName, string text)
		{
			var directoryInfo = new DirectoryInfo(folderName);
			FileInfo[] result = directoryInfo.GetFiles(SearchErrorsPattern, SearchOption.TopDirectoryOnly).OrderByDescending(x => x.LastWriteTime).ToArray();
			return string.IsNullOrWhiteSpace(text) ? result : (from fileInfo in result let itemResult = File.ReadAllText(fileInfo.FullName) where itemResult.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0 select fileInfo).ToArray();
		}

		public bool Delete(string fileName, string path = "~/Logs/Errors/")
		{
			if (string.IsNullOrWhiteSpace(fileName) || fileName == "undefined")
			{
				string folderName = HttpContext.Current.Server.MapPath(path);
				if (Directory.Exists(folderName))
				{
					var directoryInfo = new DirectoryInfo(folderName);
					FileInfo[] files = directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly).OrderByDescending(x => x.LastWriteTime).ToArray();
					foreach (FileInfo fileInfo in files)
					{
						DeleteFile(fileInfo.FullName, path);
					}
					return directoryInfo.GetFiles(SearchErrorsPattern, SearchOption.TopDirectoryOnly).Length == 0;
				}
			}
			return DeleteFile(fileName, path);
		}

		private bool DeleteFile(string fileName, string path)
		{
			string pathFile = Path.Combine(HttpContext.Current.Server.MapPath(path), fileName);
			if (File.Exists(pathFile))
				File.Delete(pathFile);
			else
				return false;
			return !File.Exists(pathFile);
		}

		public byte[] GetFilesAchive(FileInfo[] files)
		{
			using (var stream = new MemoryStream())
			{
				using (Package package = Package.Open(stream, FileMode.Create))
				{
					foreach (FileInfo fileInfo in files)
					{
						if (fileInfo.Exists)
						{
							// Добавляем документ в архив, обязательно указываем уровень сжатия.
							PackagePart packagePartDocument = package.CreatePart(PackUriHelper.CreatePartUri(new Uri(fileInfo.Name, UriKind.Relative)), MediaTypeNames.Text.Plain, CompressionOption.Normal);
							// Копируем данные документа в архив
							using (var fileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read))
							{
								try
								{
									if (packagePartDocument != null) CopyStream(fileStream, packagePartDocument.GetStream());
								}
								catch (Exception exception)
								{
									fileStream.Close();
									Logger.Implementation().Error(exception);
									return new byte[0];
								}
							}
						}
					}
				}
				return stream.GetBuffer();
			}
		}

		private static void CopyStream(Stream source, Stream target)
		{
			const int bufSize = 0x1000;
			var buf = new byte[bufSize];
			int bytesRead;
			while ((bytesRead = source.Read(buf, 0, bufSize)) > 0) target.Write(buf, 0, bytesRead);
		}

		public object GetFile(string fileName, string path = "~/Logs/Errors/")
		{
			string result = File.Exists(fileName)
					   ? File.ReadAllText(fileName)
					   : File.Exists(Path.Combine(HttpContext.Current.Server.MapPath(path), fileName))
							 ? File.ReadAllText(Path.Combine(HttpContext.Current.Server.MapPath(path), fileName)) :
							 string.Empty;
			if (string.IsNullOrWhiteSpace(result))
				throw new FileNotFoundException(string.Format("Искомый файл {0} не найден", fileName));
			return result;
		}
	}
}