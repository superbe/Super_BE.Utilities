using System.Collections.Generic;

namespace Super_BE.Utilities.Diagnostics
{
	/// <summary>
	/// Журнал ошибок.
	/// </summary>
	public class Errors
	{
		private readonly List<ErrorItem> _items;

		public Errors()
		{
			_items = new List<ErrorItem>();
		}

		public Errors(List<ErrorItem> items)
		{
			_items = items;
		}

		public List<ErrorItem> Items
		{
			get { return _items; }
		}

		public int PageCount { get; set; }
		
		public int Page { get; set; }
	}
}