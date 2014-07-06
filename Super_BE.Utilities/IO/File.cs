using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Super_BE.Utilities.IO
{
	public class File
	{
		public static FileInfo Save(string fileName, HttpPostedFileBase fileUpload)
		{
			if (fileUpload != null && fileUpload.ContentLength > 0)
			{
				if (SaveFile(fileName, fileUpload))
				{
					return new FileInfo
					{
						CreateDate = DateTime.Now,
						FileName = fileName,
						Hash = GetHash(fileName),
						Size = fileUpload.ContentLength
					};
				}
			}
			return new FileInfo();
		}

		/// <summary>
		/// Высчитать хэш-сумму файла.
		/// </summary>
		/// <param name="fileName">Имя файла.</param>
		/// <returns>Хэш-сумма файла.</returns>
		private static string GetHash(string fileName)
		{
			byte[] result = System.IO.File.ReadAllBytes(fileName);
			var hash = new SHA1Managed();
			var hashed = hash.ComputeHash(result);
			return hashed.Aggregate<byte, string>(null, (current, b) => current + string.Format("{0:x2}", b));
		}

		/// <summary>
		/// Сохранить файл.
		/// </summary>
		/// <param name="fileName">Имя файла.</param>
		/// <param name="fileUpload">Загруженный файл.</param>
		/// <returns>Результат сохранения файла.</returns>
		private static bool SaveFile(string fileName, HttpPostedFileBase fileUpload)
		{
			try
			{
				if (!string.IsNullOrEmpty(fileName))
				{
					fileUpload.SaveAs(fileName);
					return true;
				}
				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}

		///// <summary>
		///// Подготовили миниатюру.
		///// </summary>
		///// <param name="sourceFileName"></param>
		///// <param name="destinationFileName"></param>
		///// <param name="width"></param>
		///// <param name="height"></param>
		///// <remarks>
		///// Отрефакторить и перенести в другой класс.
		///// </remarks>
		///// Utility.GenerateThumbnail(Path.Combine(folderName, file.Name), Path.Combine(folderName, file.Name.Replace(file.Extension, ".png")));
		//public static void GenerateThumbnail(string sourceFileName, string destinationFileName, int width = 187, int height = 140)
		//{
		//	var videoFile = new AudioVideoFile(sourceFileName);
		//	var rand = new Random((int)DateTime.Now.Ticks);
		//	double thumbtime = rand.Next((int)videoFile.Duration.TotalSeconds);
		//	ThumbnailGenerator generator = videoFile.CreateThumbnailGenerator(new Size(width, height));
		//	Bitmap image = generator.CreateThumbnail(TimeSpan.FromSeconds(thumbtime));
		//	image.Save(destinationFileName, ImageFormat.Png);
		//	image.Dispose();
		//	generator.Dispose();
		//}
	}
}
