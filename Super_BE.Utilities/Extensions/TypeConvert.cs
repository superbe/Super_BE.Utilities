namespace Super_BE.Utilities.Extensions
{
	/// <summary>
	/// Формат сохранения сериализованного экземпляра объекта в файл.
	/// </summary>
	public enum TypeConvert
	{
		/// <summary>
		/// В строку.
		/// </summary>
		/// <remarks>Формат расширения *.txt</remarks>
		Default,
		
		/// <summary>
		/// В Json/
		/// </summary>
		/// <remarks>Формат расширения *.json</remarks>
		Json,
		
		/// <summary>
		/// В XML.
		/// </summary>
		/// <remarks>Формат расширения *.xml</remarks>
		Xml,
		
		/// <summary>
		/// Бинарная.
		/// </summary>
		/// <remarks>Формат расширения *.bn</remarks>
		Binary
	}
}