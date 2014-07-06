using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Super_BE.Utilities.Drawing
{
	/// <summary>
	/// Класс обработки загруженных изображений.
	/// </summary>
	public class Images
	{
		/// <summary>
		/// Перерисовать изображение.
		/// </summary>
		/// <param name="stream">Потоковое.</param>
		/// <param name="width">Новое значение ширины.</param>
		/// <param name="height">Новое значение высоты.</param>
		/// <param name="toGrayscale">Черно-белое.</param>
		/// <param name="toScale">Масштабировать.</param>
		/// <returns>Перерисованное изображение.</returns>
		public Image ReDrawImage(Stream stream, int width, int height, bool toScale = true, bool toGrayscale = false)
		{
			return ReDrawImage(Image.FromStream(stream), width, height, toScale, toGrayscale);
		}

		/// <summary>
		/// Перерисовать изображение.
		/// </summary>
		/// <param name="fileName">Наименование файла.</param>
		/// <param name="width">Новое значение ширины.</param>
		/// <param name="height">Новое значение высоты.</param>
		/// <param name="toGrayscale">Черно-белое.</param>
		/// <param name="toScale">Масштабировать.</param>
		/// <returns>Перерисованное изображение.</returns>
		public Image ReDrawImage(string fileName, int width, int height, bool toScale = true, bool toGrayscale = false)
		{
			return ReDrawImage(Image.FromFile(fileName), width, height, toScale, toGrayscale);
		}

		/// <summary>
		/// Перерисовать изображение.
		/// </summary>
		/// <param name="source">Исходное изображение.</param>
		/// <param name="width">Новое значение ширины.</param>
		/// <param name="height">Новое значение высоты.</param>
		/// <param name="toGrayscale">Черно-белое.</param>
		/// <param name="toScale">Масштабировать.</param>
		/// <returns>Перерисованное изображение.</returns>
		public Image ReDrawImage(Image source, int width, int height, bool toScale = true, bool toGrayscale = false)
		{
			Image result = new Bitmap(width, height);
			if (source != null)
			{
				using (Graphics picture = Graphics.FromImage(result))
				{
					picture.InterpolationMode = InterpolationMode.HighQualityBicubic;
					picture.CompositingQuality = CompositingQuality.HighQuality;
					picture.SmoothingMode = SmoothingMode.HighQuality;
					picture.FillRectangle(new SolidBrush(Color.White), -1, -1, width + 1, height + 1);
					var colorMatrix = new ColorMatrix(new[] { new[] { .3f, .3f, .3f, 0, 0 }, new[] { .59f, .59f, .59f, 0, 0 }, new[] { .11f, .11f, .11f, 0, 0 }, new float[] { 0, 0, 0, 1, 0 }, new float[] { 0, 0, 0, 0, 1 } });
					var attributes = new ImageAttributes();
					if (toGrayscale)
						attributes.SetColorMatrix(colorMatrix);
					float nPercentW = (width / (float)source.Width);
					float nPercentH = (height / (float)source.Height);
					float nPercent = nPercentH < nPercentW ? nPercentH : nPercentW;
					Rectangle rectangle = toScale
											  ? new Rectangle(
													(nPercentH < nPercentW
														? Convert.ToInt16((width - (source.Width * nPercent)) / 2)
														: 0) - 1,
													(nPercentH < nPercentW
														? 0
														: Convert.ToInt16((height - (source.Height * nPercent)) /
																				 2)) - 1, (nPercentH < nPercentW ? (int)(source.Width * nPercent) : width) + 1,
													(nPercentH < nPercentW ? height : (int)(source.Height * nPercent)) + 1)
											  : new Rectangle(0, 0, width, height);
					picture.DrawImage(source, rectangle, 0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attributes);
					picture.Dispose();
				}
			}
			return result;
		}

		/// <summary>
		/// Получить массив значений насыщенности.
		/// </summary>
		/// <param name="image">Путь к картинке.</param>
		/// <returns>Массив значений насыщенности.</returns>
		public int[] GetArray(string image)
		{
			return GetArray(new Bitmap(image));
		}

		/// <summary>
		/// Получить массив значений насыщенности.
		/// </summary>
		/// <param name="image">Изображение.</param>
		/// <returns>Массив значений насыщенности.</returns>
		public int[] GetArray(Bitmap image)
		{
			var result = new int[image.Height * image.Width];
			for (int i = 0; i < image.Height; i++)
			{
				for (int j = 0; j < image.Width; j++)
				{
					result[(i * image.Height) + j] = (int)(image.GetPixel(i, j).GetBrightness() * 255);
				}
			}
			return result;
		}

		/// <summary>
		/// Преобразовать изображение в массив байт.
		/// </summary>
		/// <param name="image">Изображение.</param>
		/// <param name="imageFormat">Формат изображения.</param>
		/// <returns>Массив байт.</returns>
		public byte[] ImageToByteArray(Image image, string imageFormat)
		{
			return ImageToByteArray(image, GetImageFormat(imageFormat));
		}

		/// <summary>
		/// Преобразовать изображение в массив байт.
		/// </summary>
		/// <param name="image">Изображение.</param>
		/// <param name="imageFormat">Формат изображения.</param>
		/// <returns>Массив байт.</returns>
		public byte[] ImageToByteArray(Image image, ImageFormat imageFormat)
		{
			MemoryStream memoryStream = new MemoryStream();
			image.Save(memoryStream, imageFormat);
			return memoryStream.ToArray();
		}

		/// <summary>
		/// Преобразовать массив байт в изображение.
		/// </summary>
		/// <param name="byteArrayIn">Массив байт.</param>
		/// <returns>Изображение.</returns>
		public Image ByteArrayToImage(byte[] byteArrayIn)
		{
			return Image.FromStream(new MemoryStream(byteArrayIn));
		}

		private static ImageFormat GetImageFormat(string imageFormat)
		{
			switch (imageFormat.ToLower())
			{
				case "bmp":
					return ImageFormat.Bmp;
				case "emf":
					return ImageFormat.Emf;
				case "exf":
				case "exif":
					return ImageFormat.Exif;
				case "gif":
					return ImageFormat.Gif;
				case "ico":
				case "icon":
					return ImageFormat.Icon;
				case "jpg":
				case "jpeg":
					return ImageFormat.Jpeg;
				case "wmf":
					return ImageFormat.Wmf;
				case "png":
					return ImageFormat.Png;
				case "tiff":
					return ImageFormat.Tiff;
				default:
					return ImageFormat.MemoryBmp;
			}
		}

		/// <summary>
		/// Преобразовать изображение в строку внедрённых данных.
		/// </summary>
		/// <param name="stream">Потоковое.</param>
		/// <param name="fileName">Наименование файла изображения.</param>
		/// <param name="width">Новое значение ширины.</param>
		/// <param name="height">Новое значение высоты.</param>
		/// <param name="toGrayscale">Черно-белое.</param>
		/// <param name="toScale">Масштабировать.</param>
		/// <returns>Строка внедрённых данных.</returns>
		public string GetDataURL(Stream stream, string fileName, int width, int height, bool toScale = true, bool toGrayscale = false)
		{
			return string.Format("data:image/{0};base64,{1}", Path.GetExtension(fileName).Replace(".", ""), Convert.ToBase64String(ImageToByteArray(ReDrawImage(stream, width, height, toScale, toGrayscale), Path.GetExtension(fileName).Replace(".", ""))));
		}

		/// <summary>
		/// Преобразовать изображение в строку внедрённых данных.
		/// </summary>
		/// <param name="stream">Потоковое.</param>
		/// <param name="fileName">Наименование файла изображения.</param>
		/// <returns>Строка внедрённых данных.</returns>
		public string GetDataURL(Stream stream, string fileName)
		{
			return string.Format("data:image/{0};base64,{1}", Path.GetExtension(fileName).Replace(".", ""), Convert.ToBase64String(ImageToByteArray(Image.FromStream(stream), Path.GetExtension(fileName).Replace(".", ""))));
		}
	}
}
