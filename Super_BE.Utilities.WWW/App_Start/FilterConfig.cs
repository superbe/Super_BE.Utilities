using System.Web;
using System.Web.Mvc;

namespace Super_BE.Utilities.WWW
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}
	}
}
