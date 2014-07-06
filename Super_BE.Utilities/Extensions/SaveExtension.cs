using System;
using System.IO;

namespace Super_BE.Utilities.Extensions
{
	public static class SaveExtension
	{
		public static string SaveAs<T>(this T instance, string fileName, SaveFormat format = SaveFormat.String, bool anyway = false)
		{
			try
			{
				switch (format)
				{
					case SaveFormat.Binary:
						File.WriteAllBytes(fileName, instance.ToBinary());
						break;
					case SaveFormat.Json:
						File.WriteAllText(fileName, instance.ToJson(anyway));
						break;
					case SaveFormat.Xml:
						instance.ToXML().Save(fileName);
						break;
					case SaveFormat.SOAP:
						instance.ToSOAP().Save(fileName);
						break;
					default:
						File.WriteAllText(fileName, instance.ToString());
						break;
				}
			}
			catch (Exception)
			{
				return string.Empty;
			}
			return fileName;
		}

		public static void Load<T>(this T instance, string fileName, SaveFormat format = SaveFormat.String)
		{
			//this = 
			//instance = 
		}
	}
}