using System;

namespace Ultra.Config.ExtensionMethods
{
	public static class DateExtensions
	{
		private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static DateTime? AsOptionalUnixTime(this double target)
		{
			return target == default(double) ? (DateTime?)null : AsUnixTime(target);
		}

		public static DateTime AsUnixTime(this double target)
		{
			return Epoch.AddSeconds(target);
		}

		public static DateTime AsUnixTime(this long target)
		{
			return ((double)target).AsUnixTime();
		}

		public static double ToUnixTime(this DateTime target)
		{
			return (target - Epoch).TotalSeconds;
		} 
	}
}