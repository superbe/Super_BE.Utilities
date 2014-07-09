namespace Super_BE.Utilities.Diagnostics
{
	/// <summary>
	/// Класс данных журнала ошибки.
	/// </summary>
	public class ErrorItem
	{
		/// <summary>
		/// Наименование файла записи журнала ошибки.
		/// </summary>
		public string FileName { get; set; }

		public string Date { get; set; }
		
		/// <summary>
		/// Содержание файла записи журнала ошибки.
		/// </summary>
		public string Context { get; set; }
	}
}