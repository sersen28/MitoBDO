using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MitoBDO.Utility
{
	public static class TimeUtil
	{
		private const string DateTimeFormat = "yyyy.MM.dd HH:mm";

		public static DateTime TimeStampToDateTime(long value)
		{
			DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			dt = dt.AddSeconds(value).ToLocalTime();
			return dt;
		}

		public static string DateTimeToString(DateTime dateTime)
		{
			return dateTime.ToString("DateTimeFormat");
		}
	}
}
