using System;

namespace Super_BE.Utilities.Diagnostics
{
	public class Attendance
	{
		public DateTime Date { get; set; }
		public int Visits { get; set; }
		public int Hosts { get; set; }
		public int Ip { get; set; }
		public int Users { get; set; }
		public string MarkerName { get; set; }

		public Attendance(DateTime date, string markerName)
		{
			Date = date;
			MarkerName = markerName;
		}

		public Attendance(HttpAggregateItem item, string markerName)
		{
			MarkerName = markerName;
			Date = item.Date;
			Visits = item.Count;
			Hosts = item.CountHosts;
			Ip = item.CountIp;
			Users = item.CountUsers;
		}
	}
}