using System.Xml.Linq;

namespace Super_BE.Utilities.Extensions
{
	/// <summary>
	/// 
	/// </summary>
	public static class XMLConvertExtension
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="instance"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static XDocument ToXML<T>(this T instance)
		{
			return new XDocument();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="document"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T FromXML<T>(this XDocument document) where T : new()
		{
			return new T();
		}
	}
}