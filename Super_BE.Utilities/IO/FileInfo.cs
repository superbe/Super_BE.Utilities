using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_BE.Utilities.IO
{
	public struct FileInfo
	{
		/// <summary>
		/// Полный путь файла.
		/// </summary>
		public string FileName;

		/// <summary>
		/// Дата создания.
		/// </summary>
		public DateTime CreateDate;

		/// <summary>
		/// Размер файла.
		/// </summary>
		public long Size;

		/// <summary>
		/// Хэш файла.
		/// </summary>
		public string Hash;
	}
}