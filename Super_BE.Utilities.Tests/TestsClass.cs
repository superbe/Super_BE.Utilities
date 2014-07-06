using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_BE.Utilities.Tests
{
	/// <summary>
	/// Тестовый класс.
	/// </summary>
	[Serializable]
	public class TestsClass
	{
		/// <summary>
		/// Уникальный идентификатор элемента.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Наименование элемента.
		/// </summary>
		public string Name { get; set; }
	}
}
